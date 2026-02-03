namespace IconGenerator.Functions;

using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using IconGenerator.Functions.Services;
using IconGenerator.Functions.Options;

public class GetUserDataFunction
{
    private readonly IDatabaseService _databaseService;
    private readonly ILogger<GetUserDataFunction> _logger;
    private readonly AppSettingsOptions _appSettings;

    public GetUserDataFunction(
        IDatabaseService databaseService,
        ILogger<GetUserDataFunction> logger,
        IOptions<AppSettingsOptions> appSettings)
    {
        _databaseService = databaseService;
        _logger = logger;
        _appSettings = appSettings.Value;
    }

    [Function("GetUserData")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "users/{userId}")] HttpRequestData req,
        string userId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get user data function triggered for user: {UserId}", userId);

        try
        {
            // Validate Authorization header
            var authHeader = req.Headers.GetValues("Authorization").FirstOrDefault();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                var unauthorizedResponse = req.CreateResponse(HttpStatusCode.Unauthorized);
                await unauthorizedResponse.WriteAsJsonAsync(new { error = "Missing or invalid authorization token" });
                return unauthorizedResponse;
            }

            if (string.IsNullOrEmpty(userId))
            {
                var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequestResponse.WriteAsJsonAsync(new { error = "User ID is required" });
                return badRequestResponse;
            }

            // Get user data
            var user = await _databaseService.GetUserAsync(userId, cancellationToken);
            if (user == null)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
                await notFoundResponse.WriteAsJsonAsync(new { error = "User not found" });
                return notFoundResponse;
            }

            // Get user's icon history
            var icons = await _databaseService.GetUserIconsAsync(userId, 50, cancellationToken);

            // Get user's transaction history
            var transactions = await _databaseService.GetUserTransactionsAsync(userId, 50, cancellationToken);

            // Check if user has unlimited access
            var isUnlimitedUser = !string.IsNullOrEmpty(user.Email) &&
                                  _appSettings.UnlimitedUsers.Contains(user.Email, StringComparer.OrdinalIgnoreCase);

            // Return response
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new
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
            }, cancellationToken);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user data");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteAsJsonAsync(new { error = "An error occurred while fetching user data" });
            return errorResponse;
        }
    }
}
