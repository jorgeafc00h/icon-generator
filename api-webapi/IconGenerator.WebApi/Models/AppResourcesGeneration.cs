namespace IconGenerator.Functions.Models;

/// <summary>
/// Represents a comprehensive app resources generation request
/// Supports icons, mockups, and design system generation
/// </summary>
public class AppResourcesGenerationRequest
{
    public string IconId { get; set; } = string.Empty;
    public List<string> Platforms { get; set; } = new();
    public ResourceType ResourceType { get; set; } = ResourceType.AppIcons;
    public AppResourcesOptions Options { get; set; } = new();
}

public enum ResourceType
{
    AppIcons,           // Standard app icons for iOS/Android/Web
    AppMockups,         // UI screen mockups
    SplashScreens,      // Launch screens
    AdaptiveIcons,      // Android adaptive icons
    DesignSystem        // Complete design system with guidelines
}

public class AppResourcesOptions
{
    // Icon Generation Options
    public bool IncludeAdaptiveIcons { get; set; } = true;
    public bool GenerateAppIconSet { get; set; } = true;
    public bool OptimizeForClarity { get; set; } = true;
    public bool GenerateDarkMode { get; set; } = false;

    // Material Design Options
    public bool GenerateMaterialYou { get; set; } = true;
    public string BackgroundColor { get; set; } = "#FFFFFF";

    // App Information
    public string? AppName { get; set; }
    public string? ThemeColor { get; set; }
    public string? BrandPrimaryColor { get; set; }
    public string? BrandSecondaryColor { get; set; }

    // Mockup Options
    public MockupStyle MockupStyle { get; set; } = MockupStyle.Modern;
    public List<ScreenType> ScreenTypes { get; set; } = new();

    // Design System Options
    public bool GenerateDesignGuide { get; set; } = false;
    public bool GenerateColorPalette { get; set; } = true;
    public bool AnalyzeDesignQuality { get; set; } = true;
}

public enum MockupStyle
{
    Modern,             // Clean, modern UI
    Minimal,            // Minimalist design
    Glassmorphism,      // Frosted glass effects
    Neumorphism,        // Soft UI
    Material3           // Material Design 3
}

public enum ScreenType
{
    Login,
    Signup,
    Dashboard,
    Profile,
    Settings,
    Onboarding,
    Feed,
    Detail,
    Checkout,
    Search
}

/// <summary>
/// Response containing generated app resources
/// </summary>
public class AppResourcesGenerationResponse
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ZipUrl { get; set; } = string.Empty;
    public string? DesignGuideUrl { get; set; }
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(7);

    public long FileSize { get; set; }
    public int TotalAssets { get; set; }
    public List<string> Platforms { get; set; } = new();

    // Design Analysis
    public DesignQualityScore? DesignScore { get; set; }
    public ColorPalette? ExtractedColors { get; set; }
    public List<string> GeneratedSizes { get; set; } = new();
}

/// <summary>
/// Design quality scoring
/// </summary>
public class DesignQualityScore
{
    public int OverallScore { get; set; } // 0-100
    public int ClarityScore { get; set; }
    public int ContrastScore { get; set; }
    public int ScalabilityScore { get; set; }
    public int AccessibilityScore { get; set; }

    public bool MeetsWCAG_AA { get; set; }
    public bool MeetsWCAG_AAA { get; set; }

    public List<DesignIssue> Issues { get; set; } = new();
    public List<DesignSuggestion> Suggestions { get; set; } = new();
}

public class DesignIssue
{
    public IssueSeverity Severity { get; set; }
    public IssueCategory Category { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Fix { get; set; } = string.Empty;
}

public enum IssueSeverity
{
    Info,
    Warning,
    Critical
}

public enum IssueCategory
{
    Accessibility,
    Clarity,
    Platform,
    Contrast,
    Scalability
}

public class DesignSuggestion
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public SuggestionImpact Impact { get; set; }
    public SuggestionEffort Effort { get; set; }
    public IssueCategory Category { get; set; }
}

public enum SuggestionImpact
{
    Low,
    Medium,
    High
}

public enum SuggestionEffort
{
    Low,
    Medium,
    High
}

/// <summary>
/// Extracted color palette from icon
/// </summary>
public class ColorPalette
{
    public string Dominant { get; set; } = string.Empty;
    public string Vibrant { get; set; } = string.Empty;
    public string DarkVibrant { get; set; } = string.Empty;
    public string LightVibrant { get; set; } = string.Empty;
    public string Muted { get; set; } = string.Empty;
    public string DarkMuted { get; set; } = string.Empty;
    public string LightMuted { get; set; } = string.Empty;

    public MaterialYouScheme? MaterialYou { get; set; }
    public IOSColorScheme? IOS { get; set; }
}

public class MaterialYouScheme
{
    public string Primary { get; set; } = string.Empty;
    public string OnPrimary { get; set; } = string.Empty;
    public string PrimaryContainer { get; set; } = string.Empty;
    public string OnPrimaryContainer { get; set; } = string.Empty;

    public string Secondary { get; set; } = string.Empty;
    public string OnSecondary { get; set; } = string.Empty;
    public string SecondaryContainer { get; set; } = string.Empty;

    public string Tertiary { get; set; } = string.Empty;
    public string Error { get; set; } = "#DC362E";

    public string Background { get; set; } = "#FEFBFF";
    public string Surface { get; set; } = "#FEFBFF";
    public string SurfaceVariant { get; set; } = "#E7E0EC";
    public string Outline { get; set; } = "#79747E";
}

public class IOSColorScheme
{
    public string Tint { get; set; } = string.Empty;
    public Dictionary<string, string> LightMode { get; set; } = new();
    public Dictionary<string, string> DarkMode { get; set; } = new();
}

/// <summary>
/// App mockup generation request
/// </summary>
public class AppMockupGenerationRequest
{
    public string AppName { get; set; } = "My App";
    public string Description { get; set; } = string.Empty;
    public ScreenType ScreenType { get; set; }
    public MockupStyle Style { get; set; } = MockupStyle.Modern;

    public List<string> BrandColors { get; set; } = new();
    public string? LogoUrl { get; set; }

    // UI Elements
    public bool IncludeHeader { get; set; } = true;
    public bool IncludeNavigation { get; set; } = true;
    public string DeviceType { get; set; } = "mobile"; // mobile, tablet, desktop
}

public class AppMockupGenerationResponse
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ImageUrl { get; set; } = string.Empty;
    public string StorageUrl { get; set; } = string.Empty;
    public ScreenType ScreenType { get; set; }
    public string EnhancedPrompt { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
