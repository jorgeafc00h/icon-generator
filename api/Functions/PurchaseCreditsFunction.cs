namespace IconGenerator.Functions;

using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using IconGenerator.Functions.Models;
using IconGenerator.Functions.Services;

public class PurchaseCreditsFunction
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PurchaseCreditsFunction> _logger;

    public PurchaseCreditsFunction(
        IPaymentService paymentService,
        ILogger<PurchaseCreditsFunction> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    [Function("CreateCheckoutSession")]
    public async Task<HttpResponseData> CreateCheckoutSession(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "payments/checkout")] HttpRequestData req,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Create checkout session function triggered");

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

            // Parse request body
            var request = await req.ReadFromJsonAsync<PurchaseRequest>(cancellationToken);
            if (request == null || string.IsNullOrEmpty(request.PackageId))
            {
                var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequestResponse.WriteAsJsonAsync(new { error = "Invalid request body" });
                return badRequestResponse;
            }

            // Create checkout session
            var session = await _paymentService.CreateCheckoutSessionAsync(userId, request, cancellationToken);

            // Return response
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(session, cancellationToken);
            return response;
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid package ID");
            var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequestResponse.WriteAsJsonAsync(new { error = ex.Message });
            return badRequestResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating checkout session");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteAsJsonAsync(new { error = "An error occurred while creating checkout session" });
            return errorResponse;
        }
    }

    [Function("GetCreditPackages")]
    public async Task<HttpResponseData> GetCreditPackages(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "payments/packages")] HttpRequestData req,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get credit packages function triggered");

        try
        {
            var packages = _paymentService.GetAllCreditPackages();

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(packages, cancellationToken);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting credit packages");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteAsJsonAsync(new { error = "An error occurred while fetching credit packages" });
            return errorResponse;
        }
    }
}
