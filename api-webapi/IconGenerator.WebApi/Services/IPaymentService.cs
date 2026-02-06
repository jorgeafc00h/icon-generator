namespace IconGenerator.Functions.Services;

using IconGenerator.Functions.Models;

public interface IPaymentService
{
    /// <summary>
    /// Create a checkout session for purchasing credits
    /// </summary>
    Task<PurchaseResponse> CreateCheckoutSessionAsync(
        string userId,
        PurchaseRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Process a successful payment webhook
    /// </summary>
    Task<bool> ProcessWebhookAsync(
        string payload,
        string signature,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get credit package by ID
    /// </summary>
    CreditPackage? GetCreditPackage(string packageId);

    /// <summary>
    /// Get all available credit packages
    /// </summary>
    List<CreditPackage> GetAllCreditPackages();
}
