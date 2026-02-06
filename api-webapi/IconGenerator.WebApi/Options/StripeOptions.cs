namespace IconGenerator.Functions.Options;

public class StripeOptions
{
    public string SecretKey { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;
    public string FrontendUrl { get; set; } = "http://localhost:5173";
}
