namespace IconGenerator.Functions.Services;

using Stripe;
using Stripe.Checkout;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using IconGenerator.Functions.Models;
using IconGenerator.Functions.Options;
using IconGenerator.Functions.Constants;

public class StripePaymentService : IPaymentService
{
    private readonly StripeOptions _options;
    private readonly IDatabaseService _databaseService;
    private readonly ILogger<StripePaymentService> _logger;

    public StripePaymentService(
        IOptions<StripeOptions> options,
        IDatabaseService databaseService,
        ILogger<StripePaymentService> logger)
    {
        _options = options.Value;
        _databaseService = databaseService;
        _logger = logger;

        StripeConfiguration.ApiKey = _options.SecretKey;
    }

    public async Task<PurchaseResponse> CreateCheckoutSessionAsync(
        string userId,
        PurchaseRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var package = GetCreditPackage(request.PackageId);
            if (package == null)
            {
                throw new ArgumentException($"Invalid package ID: {request.PackageId}");
            }

            var sessionOptions = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Price = package.StripePriceId,
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                SuccessUrl = request.SuccessUrl ?? $"{_options.FrontendUrl}/success?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = request.CancelUrl ?? $"{_options.FrontendUrl}/cancel",
                ClientReferenceId = userId,
                Metadata = new Dictionary<string, string>
                {
                    { "userId", userId },
                    { "packageId", package.Id },
                    { "credits", package.Credits.ToString() },
                    { "bonusCredits", package.BonusCredits.ToString() }
                }
            };

            var service = new SessionService();
            var session = await service.CreateAsync(sessionOptions, cancellationToken: cancellationToken);

            _logger.LogInformation("Created checkout session {SessionId} for user {UserId}", session.Id, userId);

            return new PurchaseResponse
            {
                SessionId = session.Id,
                CheckoutUrl = session.Url
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating checkout session for user {UserId}", userId);
            throw;
        }
    }

    public async Task<bool> ProcessWebhookAsync(
        string payload,
        string signature,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var webhookSecret = _options.WebhookSecret;
            var stripeEvent = EventUtility.ConstructEvent(
                payload,
                signature,
                webhookSecret
            );

            _logger.LogInformation("Processing webhook event: {EventType}", stripeEvent.Type);

            if (stripeEvent.Type == Events.CheckoutSessionCompleted)
            {
                var session = stripeEvent.Data.Object as Session;
                if (session == null)
                {
                    _logger.LogWarning("Could not parse checkout session from webhook");
                    return false;
                }

                return await ProcessSuccessfulPaymentAsync(session, cancellationToken);
            }

            return true;
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe webhook error");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing webhook");
            throw;
        }
    }

    public CreditPackage? GetCreditPackage(string packageId)
    {
        return CreditPackages.Packages.FirstOrDefault(p => p.Id == packageId);
    }

    public List<CreditPackage> GetAllCreditPackages()
    {
        return CreditPackages.Packages;
    }

    private async Task<bool> ProcessSuccessfulPaymentAsync(
        Session session,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = session.Metadata?["userId"];
            var packageId = session.Metadata?["packageId"];
            var creditsStr = session.Metadata?["credits"];
            var bonusCreditsStr = session.Metadata?["bonusCredits"];

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(creditsStr))
            {
                _logger.LogWarning("Missing metadata in checkout session {SessionId}", session.Id);
                return false;
            }

            if (!int.TryParse(creditsStr, out var credits))
            {
                _logger.LogWarning("Invalid credits value in session {SessionId}", session.Id);
                return false;
            }

            // Parse bonus credits (default to 0 if not present or invalid)
            if (!int.TryParse(bonusCreditsStr, out var bonusCredits))
            {
                bonusCredits = 0;
            }

            // Calculate total credits (base + bonus)
            var totalCredits = credits + bonusCredits;

            // Add credits to user
            var user = await _databaseService.AddCreditsAsync(userId, totalCredits, cancellationToken);

            // Save transaction record
            var transactionDescription = bonusCredits > 0
                ? $"Purchased {credits} credits + {bonusCredits} bonus credits"
                : $"Purchased {credits} credits";

            var transaction = new Transaction
            {
                UserId = userId,
                Type = "purchase",
                Credits = totalCredits,
                AmountInCents = (int?)session.AmountTotal,
                Description = transactionDescription,
                StripeSessionId = session.Id
            };

            await _databaseService.SaveTransactionAsync(transaction, cancellationToken);

            _logger.LogInformation(
                "Added {TotalCredits} credits ({Credits} base + {BonusCredits} bonus) to user {UserId} from session {SessionId}",
                totalCredits, credits, bonusCredits, userId, session.Id);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing successful payment for session {SessionId}", session.Id);
            return false;
        }
    }
}
