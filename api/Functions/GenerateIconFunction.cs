namespace IconGenerator.Functions;

using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using IconGenerator.Functions.Models;
using IconGenerator.Functions.Services;

public class GenerateIconFunction
{
    private readonly IAIService _aiService;
    private readonly IStorageService _storageService;
    private readonly IDatabaseService _databaseService;
    private readonly ILogger<GenerateIconFunction> _logger;

    public GenerateIconFunction(
        IAIService aiService,
        IStorageService storageService,
        IDatabaseService databaseService,
        ILogger<GenerateIconFunction> logger)
    {
        _aiService = aiService;
        _storageService = storageService;
        _databaseService = databaseService;
        _logger = logger;
    }

    [Function("GenerateIcon")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "icons/generate")] HttpRequestData req,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generate icon function triggered");

        try
        {
            // Get user ID from headers (set by authentication middleware)
            var userId = req.Headers.GetValues("X-User-Id").FirstOrDefault() ?? "anonymous";

            // Parse request body
            var request = await req.ReadFromJsonAsync<IconGenerationRequest>(cancellationToken);
            if (request == null)
            {
                return await CreateErrorResponse(req, HttpStatusCode.BadRequest, "Invalid request body");
            }

            // Validate input
            if (string.IsNullOrWhiteSpace(request.Keywords))
            {
                return await CreateErrorResponse(req, HttpStatusCode.BadRequest, "Keywords are required");
            }

            if (string.IsNullOrWhiteSpace(request.Style))
            {
                return await CreateErrorResponse(req, HttpStatusCode.BadRequest, "Style is required");
            }

            if (request.Colors == null || request.Colors.Count == 0)
            {
                return await CreateErrorResponse(req, HttpStatusCode.BadRequest, "At least one color is required");
            }

            // Check and deduct credits
            var hasCredits = await _databaseService.DeductCreditsAsync(userId, 1, cancellationToken);
            if (!hasCredits)
            {
                return await CreateErrorResponse(req, HttpStatusCode.PaymentRequired, "Insufficient credits");
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

                // Record transaction
                var transaction = new Transaction
                {
                    UserId = userId,
                    Type = "usage",
                    Credits = -1,
                    Description = $"Generated icon: {request.Keywords}"
                };
                await _databaseService.SaveTransactionAsync(transaction, cancellationToken);

                // Get updated user data
                var user = await _databaseService.GetUserAsync(userId, cancellationToken);
                var creditsRemaining = user?.Credits ?? 0;

                // Return response
                var response = new IconGenerationResponse
                {
                    IconId = iconId,
                    ImageUrl = storedUrl,
                    EnhancedPrompt = enhancedPrompt,
                    CreditsRemaining = creditsRemaining
                };

                var httpResponse = req.CreateResponse(HttpStatusCode.OK);
                await httpResponse.WriteAsJsonAsync(response, cancellationToken);
                return httpResponse;
            }
            catch (Exception ex)
            {
                // Refund credit on error
                _logger.LogError(ex, "Error generating icon, refunding credit");
                await _databaseService.AddCreditsAsync(userId, 1, cancellationToken);
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GenerateIcon function");
            return await CreateErrorResponse(req, HttpStatusCode.InternalServerError, "An error occurred while generating the icon");
        }
    }

    private async Task<HttpResponseData> CreateErrorResponse(HttpRequestData req, HttpStatusCode statusCode, string message)
    {
        var response = req.CreateResponse(statusCode);
        await response.WriteAsJsonAsync(new { error = message });
        return response;
    }
}
