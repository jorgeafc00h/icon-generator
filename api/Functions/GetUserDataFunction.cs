namespace IconGenerator.Functions;

using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using IconGenerator.Functions.Services;

public class GetUserDataFunction
{
    private readonly IDatabaseService _databaseService;
    private readonly ILogger<GetUserDataFunction> _logger;

    public GetUserDataFunction(
        IDatabaseService databaseService,
        ILogger<GetUserDataFunction> logger)
    {
        _databaseService = databaseService;
        _logger = logger;
    }

    [Function("GetUserData")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "user/data")] HttpRequestData req,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get user data function triggered");

        try
        {
            // Get user ID from headers
            var userId = req.Headers.GetValues("X-User-Id").FirstOrDefault();

            if (string.IsNullOrEmpty(userId))
            {
                var errorResponse = req.CreateResponse(HttpStatusCode.Unauthorized);
                await errorResponse.WriteAsJsonAsync(new { error = "User ID not found" });
                return errorResponse;
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

            // Return response
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new
            {
                user = new
                {
                    id = user.Id,
                    email = user.Email,
                    credits = user.Credits,
                    createdAt = user.CreatedAt,
                    metadata = user.Metadata
                },
                icons = icons.Select(i => new
                {
                    id = i.Id,
                    imageUrl = i.ImageUrl,
                    prompt = i.Prompt,
                    style = i.Style,
                    colors = i.Colors,
                    quality = i.Quality,
                    createdAt = i.CreatedAt
                }),
                transactions = transactions.Select(t => new
                {
                    id = t.Id,
                    type = t.Type,
                    credits = t.Credits,
                    amountInCents = t.AmountInCents,
                    description = t.Description,
                    createdAt = t.CreatedAt
                })
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
