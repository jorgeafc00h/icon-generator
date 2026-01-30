namespace IconGenerator.Functions.Models;

public class AssetGeneration
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = string.Empty;
    public string IconId { get; set; } = string.Empty;
    public List<string> Platforms { get; set; } = new();
    public string ZipUrl { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public int TotalAssets { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(7);
}

public class AssetGenerationRequest
{
    public string IconId { get; set; } = string.Empty;
    public List<string> Platforms { get; set; } = new();
    public AssetGenerationOptions? Options { get; set; }
}

public class AssetGenerationOptions
{
    public bool IncludeAdaptiveIcons { get; set; } = true;
    public bool GenerateAppIconSet { get; set; } = true;
    public bool RoundedCorners { get; set; }
    public string BackgroundColor { get; set; } = "#FFFFFF";
    public string? AppName { get; set; }
    public string? ThemeColor { get; set; }
}

public class AssetGenerationResponse
{
    public string ZipUrl { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public long FileSize { get; set; }
    public List<string> Platforms { get; set; } = new();
    public int TotalAssets { get; set; }
}
