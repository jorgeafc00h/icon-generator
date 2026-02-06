namespace IconGenerator.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Text;
using IconGenerator.Functions.Services;

[ApiController]
[Route("api/webhooks")]
public class WebhooksController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<WebhooksController> _logger;

    public WebhooksController(
        IPaymentService paymentService,
        ILogger<WebhooksController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    /// <summary>
    /// Stripe webhook endpoint for payment events
    /// </summary>
    [HttpPost("stripe")]
    public async Task<IActionResult> StripeWebhook(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stripe webhook endpoint triggered");

        try
        {
            // Get the request body
            string requestBody;
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            // Get the Stripe signature header
            var signature = Request.Headers["Stripe-Signature"].FirstOrDefault();

            if (string.IsNullOrEmpty(signature))
            {
                _logger.LogWarning("Stripe webhook called without signature");
                return BadRequest(new { error = "Missing Stripe signature" });
            }

            // Process the webhook
            var success = await _paymentService.ProcessWebhookAsync(requestBody, signature, cancellationToken);

            if (success)
            {
                return Ok(new { received = true });
            }
            else
            {
                _logger.LogWarning("Webhook processing returned false");
                return BadRequest(new { error = "Webhook processing failed" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing Stripe webhook");

            // Still return 200 to acknowledge receipt, but log the error
            // Stripe will retry if we return non-2xx
            return Ok(new { received = true, error = "Internal processing error" });
        }
    }
}
