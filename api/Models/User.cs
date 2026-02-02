namespace IconGenerator.Functions.Models;

public class User
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Email { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public int Credits { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public UserAuth? Auth { get; set; }
    public UserMetadata? Metadata { get; set; }
    public UserPreferences? Preferences { get; set; }
}

public class UserAuth
{
    public string? GoogleId { get; set; }
    public string? GoogleEmail { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public string? RefreshToken { get; set; }
}

public class UserMetadata
{
    public DateTime? LastIconGenerated { get; set; }
    public int TotalIconsGenerated { get; set; }
    public int TotalCreditsPurchased { get; set; }
    public int TotalCreditsSpent { get; set; }
}

public class UserPreferences
{
    public string? DefaultStyle { get; set; }
    public List<string>? FavoriteColors { get; set; }
    public string? DefaultQuality { get; set; } = "standard";
    public bool EmailNotifications { get; set; } = true;
}
