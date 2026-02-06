namespace IconGenerator.WebApi.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using IconGenerator.Functions.Models;
using IconGenerator.Functions.Services;
using IconGenerator.Functions.Options;

[ApiController]
[Route("api/icons")]
[Authorize]
public class IconsController : ControllerBase
{
    private readonly IAIService _aiService;
    private readonly IStorageService _storageService;
    private readonly IDatabaseService _databaseService;
    private readonly ILogger<IconsController> _logger;
    private readonly AppSettingsOptions _appSettings;

    public IconsController(
        IAIService aiService,
        IStorageService storageService,
        IDatabaseService databaseService,
        ILogger<IconsController> logger,
        IOptions<AppSettingsOptions> appSettings)
    {
        _aiService = aiService;
        _storageService = storageService;
        _databaseService = databaseService;
        _logger = logger;
        _appSettings = appSettings.Value;
    }

    /// <summary>
    /// Generate a new app icon
    /// </summary>
    [HttpPost("generate")]
    public async Task<IActionResult> GenerateIcon([FromBody] IconGenerationRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generate icon endpoint triggered");

        try
        {
            // Get user ID from JWT claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "Invalid authentication token" });
            }

            // Validate input
            if (request == null)
            {
                return BadRequest(new { error = "Invalid request body" });
            }

            if (string.IsNullOrWhiteSpace(request.Keywords))
            {
                return BadRequest(new { error = "Keywords are required" });
            }

            if (string.IsNullOrWhiteSpace(request.Style))
            {
                return BadRequest(new { error = "Style is required" });
            }

            if (request.Colors == null || request.Colors.Count == 0)
            {
                return BadRequest(new { error = "At least one color is required" });
            }

            // Check if user has unlimited access
            var user = await _databaseService.GetUserAsync(userId, cancellationToken);
            var isUnlimitedUser = user != null &&
                                  !string.IsNullOrEmpty(user.Email) &&
                                  _appSettings.UnlimitedUsers.Contains(user.Email, StringComparer.OrdinalIgnoreCase);

            // Check and deduct credits (skip for unlimited users)
            if (!isUnlimitedUser)
            {
                var hasCredits = await _databaseService.DeductCreditsAsync(userId, 1, cancellationToken);
                if (!hasCredits)
                {
                    return StatusCode(402, new { error = "Insufficient credits" });
                }
            }
            else
            {
                _logger.LogInformation("Unlimited user {Email} generating icon without credit deduction", user.Email);
            }

            try
            {
                // Step 1: Enhance prompt using GPT-4o-mini
                _logger.LogInformation("Enhancing prompt for user {UserId}", userId);
                var enhancedPrompt = await _aiService.EnhancePromptAsync(request, cancellationToken);

                // Step 2: Generate icon using DALL-E 3
                _logger.LogInformation("Generating icon with DALL-E 3 for user {UserId}", userId);
                var imageUrl = await _aiService.GenerateIconAsync(enhancedPrompt, request.Quality, cancellationToken);

                // Step 3: Upload to blob storage
                var iconId = Guid.NewGuid().ToString();
                _logger.LogInformation("Uploading icon {IconId} to blob storage", iconId);
                var storedUrl = await _storageService.UploadImageAsync(imageUrl, userId, iconId, cancellationToken);

                // Step 4: Save to database
                var iconGeneration = new IconGeneration
                {
                    Id = iconId,
                    UserId = userId,
                    Prompt = request.Keywords,
                    EnhancedPrompt = enhancedPrompt,
                    Style = request.Style,
                    Colors = request.Colors,
                    ImageUrl = storedUrl,
                    Quality = request.Quality,
                    CreatedAt = DateTime.UtcNow
                };

                await _databaseService.SaveIconGenerationAsync(iconGeneration, cancellationToken);

                // Record transaction (only for non-unlimited users)
                if (!isUnlimitedUser)
                {
                    var transaction = new Transaction
                    {
                        UserId = userId,
                        Type = "usage",
                        Credits = -1,
                        Description = $"Generated icon: {request.Keywords}"
                    };
                    await _databaseService.SaveTransactionAsync(transaction, cancellationToken);
                }

                // Get updated user data
                var updatedUser = await _databaseService.GetUserAsync(userId, cancellationToken);
                var creditsRemaining = isUnlimitedUser ? int.MaxValue : (updatedUser?.Credits ?? 0);

                // Return response
                var response = new IconGenerationResponse
                {
                    IconId = iconId,
                    ImageUrl = storedUrl,
                    EnhancedPrompt = enhancedPrompt,
                    CreditsRemaining = creditsRemaining
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Refund credit on error (only for non-unlimited users)
                if (!isUnlimitedUser)
                {
                    _logger.LogError(ex, "Error generating icon, refunding credit");
                    await _databaseService.AddCreditsAsync(userId, 1, cancellationToken);
                }
                else
                {
                    _logger.LogError(ex, "Error generating icon for unlimited user {Email}", user?.Email);
                }
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GenerateIcon endpoint");
            return StatusCode(500, new { error = "An error occurred while generating the icon" });
        }
    }
}
