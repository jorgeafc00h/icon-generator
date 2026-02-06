namespace IconGenerator.WebApi.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using IconGenerator.Functions.Models;
using IconGenerator.Functions.Services;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(
        IPaymentService paymentService,
        ILogger<PaymentsController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    /// <summary>
    /// Create Stripe checkout session for credit purchase
    /// </summary>
    [HttpPost("checkout")]
    [Authorize]
    public async Task<IActionResult> CreateCheckoutSession([FromBody] PurchaseRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Create checkout session endpoint triggered");

        try
        {
            // Get user ID from JWT claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "Invalid authentication token" });
            }

            // Validate request
            if (request == null || string.IsNullOrEmpty(request.PackageId))
            {
                return BadRequest(new { error = "Invalid request body. PackageId is required." });
            }

            // Override userId from request with authenticated user ID (security)
            request.UserId = userId;

            // Create checkout session
            var session = await _paymentService.CreateCheckoutSessionAsync(userId, request, cancellationToken);

            return Ok(session);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid package ID");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating checkout session");
            return StatusCode(500, new { error = "An error occurred while creating checkout session" });
        }
    }

    /// <summary>
    /// Get all available credit packages
    /// </summary>
    [HttpGet("packages")]
    [AllowAnonymous]
    public IActionResult GetCreditPackages()
    {
        _logger.LogInformation("Get credit packages endpoint triggered");

        try
        {
            var packages = _paymentService.GetAllCreditPackages();
            return Ok(packages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting credit packages");
            return StatusCode(500, new { error = "An error occurred while fetching credit packages" });
        }
    }
}
