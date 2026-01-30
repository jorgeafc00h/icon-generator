namespace IconGenerator.Functions.Models;

public class IconGeneration
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public string Prompt { get; set; } = string.Empty;
    public string EnhancedPrompt { get; set; } = string.Empty;
    public string Style { get; set; } = string.Empty;
    public List<string> Colors { get; set; } = new();
    public string ImageUrl { get; set; } = string.Empty;
    public string Quality { get; set; } = "standard";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class IconGenerationRequest
{
    public string Keywords { get; set; } = string.Empty;
    public string Style { get; set; } = string.Empty;
    public List<string> Colors { get; set; } = new();
    public string Quality { get; set; } = "standard";
}

public class IconGenerationResponse
{
    public string IconId { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string EnhancedPrompt { get; set; } = string.Empty;
    public int CreditsRemaining { get; set; }
}
