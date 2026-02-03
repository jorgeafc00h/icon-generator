namespace IconGenerator.Functions.Models;

public class CreditPackage
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Credits { get; set; }
    public int BonusCredits { get; set; }
    public int PriceInCents { get; set; }
    public string StripePriceId { get; set; } = string.Empty;
}

public class PurchaseRequest
{
    public string UserId { get; set; } = string.Empty;
    public string PackageId { get; set; } = string.Empty;
    public string? SuccessUrl { get; set; }
    public string? CancelUrl { get; set; }
}

public class PurchaseResponse
{
    public string SessionId { get; set; } = string.Empty;
    public string CheckoutUrl { get; set; } = string.Empty;
}

public class Transaction
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public string Type { get; set; } = "purchase"; // purchase or usage
    public int Credits { get; set; }
    public int? AmountInCents { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? StripeSessionId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
