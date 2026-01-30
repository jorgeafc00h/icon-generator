namespace IconGenerator.Functions;

using System.Net;
using System.Text;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using IconGenerator.Functions.Services;

public class StripeWebhookFunction
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<StripeWebhookFunction> _logger;

    public StripeWebhookFunction(
        IPaymentService paymentService,
        ILogger<StripeWebhookFunction> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    [Function("StripeWebhook")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "webhooks/stripe")] HttpRequestData req,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stripe webhook function triggered");

        try
        {
            // Get the request body
            string requestBody;
            using (var reader = new StreamReader(req.Body, Encoding.UTF8))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            // Get the Stripe signature header
            var signature = req.Headers.GetValues("Stripe-Signature").FirstOrDefault();

            if (string.IsNullOrEmpty(signature))
            {
                _logger.LogWarning("Stripe webhook called without signature");
                var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await errorResponse.WriteAsJsonAsync(new { error = "Missing Stripe signature" });
                return errorResponse;
            }

            // Process the webhook
            var success = await _paymentService.ProcessWebhookAsync(requestBody, signature, cancellationToken);

            if (success)
            {
                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(new { received = true });
                return response;
            }
            else
            {
                _logger.LogWarning("Webhook processing returned false");
                var failResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await failResponse.WriteAsJsonAsync(new { error = "Webhook processing failed" });
                return failResponse;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing Stripe webhook");

            // Still return 200 to acknowledge receipt, but log the error
            // Stripe will retry if we return non-2xx
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new { received = true, error = "Internal processing error" });
            return response;
        }
    }
}
