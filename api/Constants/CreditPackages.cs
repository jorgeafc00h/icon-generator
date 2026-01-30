namespace IconGenerator.Functions.Constants;

using IconGenerator.Functions.Models;

public static class CreditPackages
{
    public static readonly List<CreditPackage> Packages = new()
    {
        new CreditPackage
        {
            Id = "starter",
            Name = "Starter Pack",
            Credits = 10,
            PriceInCents = 1200, // $12
            StripePriceId = "price_starter" // Replace with actual Stripe Price ID
        },
        new CreditPackage
        {
            Id = "pro",
            Name = "Pro Pack",
            Credits = 50,
            PriceInCents = 2900, // $29
            StripePriceId = "price_pro" // Replace with actual Stripe Price ID
        },
        new CreditPackage
        {
            Id = "business",
            Name = "Business Pack",
            Credits = 150,
            PriceInCents = 4900, // $49
            StripePriceId = "price_business" // Replace with actual Stripe Price ID
        }
    };

    public static CreditPackage? GetPackage(string packageId)
    {
        return Packages.FirstOrDefault(p => p.Id == packageId);
    }
}
