namespace IconGenerator.WebApi.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using IconGenerator.Functions.Services;
using IconGenerator.Functions.Options;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IDatabaseService _databaseService;
    private readonly ILogger<UsersController> _logger;
    private readonly AppSettingsOptions _appSettings;

    public UsersController(
        IDatabaseService databaseService,
        ILogger<UsersController> logger,
        IOptions<AppSettingsOptions> appSettings)
    {
        _databaseService = databaseService;
        _logger = logger;
        _appSettings = appSettings.Value;
    }

    /// <summary>
    /// Get user data including icons and transaction history
    /// </summary>
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserData(string userId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get user data endpoint triggered for user: {UserId}", userId);

        try
        {
            // Verify the requesting user matches the userId parameter (or is admin)
            var requestingUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(requestingUserId) || requestingUserId != userId)
            {
                return Unauthorized(new { error = "You can only access your own user data" });
            }

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { error = "User ID is required" });
            }

            // Get user data
            var user = await _databaseService.GetUserAsync(userId, cancellationToken);
            if (user == null)
            {
                return NotFound(new { error = "User not found" });
            }

            // Get user's icon history
            var icons = await _databaseService.GetUserIconsAsync(userId, 50, cancellationToken);

            // Get user's transaction history
            var transactions = await _databaseService.GetUserTransactionsAsync(userId, 50, cancellationToken);

            // Check if user has unlimited access
            var isUnlimitedUser = !string.IsNullOrEmpty(user.Email) &&
                                  _appSettings.UnlimitedUsers.Contains(user.Email, StringComparer.OrdinalIgnoreCase);

            // Return response
            return Ok(new
            {
                id = user.Id,
                email = user.Email,
                name = user.Name,
                profilePictureUrl = user.ProfilePictureUrl,
                credits = user.Credits,
                isUnlimited = isUnlimitedUser,
                createdAt = user.CreatedAt,
                updatedAt = user.UpdatedAt,
                metadata = user.Metadata,
                preferences = user.Preferences,
                recentIcons = icons.Select(i => new
                {
                    id = i.Id,
                    imageUrl = i.ImageUrl,
                    prompt = i.Prompt,
                    style = i.Style,
                    colors = i.Colors,
                    quality = i.Quality,
                    createdAt = i.CreatedAt
                }).Take(10),
                recentTransactions = transactions.Select(t => new
                {
                    id = t.Id,
                    type = t.Type,
                    credits = t.Credits,
                    amountInCents = t.AmountInCents,
                    description = t.Description,
                    createdAt = t.CreatedAt
                }).Take(10)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user data");
            return StatusCode(500, new { error = "An error occurred while fetching user data" });
        }
    }
}
