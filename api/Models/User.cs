namespace IconGenerator.Functions.Models;

public class User
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Email { get; set; } = string.Empty;
    public int Credits { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public UserMetadata? Metadata { get; set; }
}

public class UserMetadata
{
    public DateTime? LastIconGenerated { get; set; }
    public int TotalIconsGenerated { get; set; }
}
