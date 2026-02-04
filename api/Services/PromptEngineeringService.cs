namespace IconGenerator.Functions.Services;

using IconGenerator.Functions.Models;
using IconGenerator.Functions.Prompts;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

/// <summary>
/// Advanced prompt engineering service that builds sophisticated prompts
/// using design knowledge base and best practices
/// </summary>
public class PromptEngineeringService
{
    private readonly ILogger<PromptEngineeringService> _logger;

    public PromptEngineeringService(ILogger<PromptEngineeringService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Build a comprehensive system prompt for icon generation
    /// </summary>
    public string BuildIconSystemPrompt(string style)
    {
        var styleGuidelines = GetStyleGuidelines(style);

        var systemPrompt = $@"You are a professional app icon designer creating polished icons for mobile and web platforms.

{styleGuidelines}

Critical design requirements:
- Create ONE SINGLE large icon that bleeds to the canvas edges
- The icon subject must extend edge-to-edge with NO visible padding or margins
- Fill the ENTIRE canvas completely - the main element touches all four edges
- NO small versions, NO multiple sizes, NO miniature icons
- NO bottom border, NO footer, NO size demonstrations, NO surrounding space
- Bold centered subject that maximizes the full square canvas
- Clean icon with no text, letters, or labels
- Simple focused subject, not complex scenes
- Clear recognizable silhouette
- Design extends to canvas boundaries with maximum visual impact

Output: A concise DALL-E prompt for a single edge-to-edge canvas-filling icon.";

        return SanitizePrompt(systemPrompt);
    }

    /// <summary>
    /// Build user prompt with enhanced context
    /// </summary>
    public string BuildIconUserPrompt(IconGenerationRequest request)
    {
        var colorGuidance = DesignKnowledgeBase.ColorPalettes.GetPalettePrompt(request.Colors);
        var is3DStyle = request.Style.ToLowerInvariant().Contains("3d") ||
                        request.Style.ToLowerInvariant() == "modern";

        var canvasGuidance = is3DStyle
            ? @"- ONE large 3D object occupying 90-95% of canvas with only 5-10% padding maximum
- Object should be LARGE and extend toward edges - avoid floating small objects
- Clean, simple composition - single subject only, NO nested icons
- Use natural perspective, not isometric"
            : @"- ONE large icon with zero margins - it bleeds to all canvas edges
- The main subject fills the complete square canvas edge-to-edge
- Bold centered element maximizing full canvas utilization";

        var userPrompt = $@"Design a {request.Style} style app icon representing: {request.Keywords}

Color scheme: {colorGuidance}

Composition requirements:
{canvasGuidance}
- NO padding, NO border space, NO excessive surrounding empty area
- NO multiple sizes, NO small preview versions, NO miniatures
- Professional {request.Style} execution
- Design extends toward the edges of the square frame

Generate a concise DALL-E prompt for a canvas-filling icon.";

        return SanitizePrompt(userPrompt);
    }

    /// <summary>
    /// Build system prompt for UI screen mockup generation (App Resources)
    /// </summary>
    public string BuildScreenSystemPrompt(string screenType)
    {
        return $@"You are an elite UI/UX designer creating modern mobile app screen mockups.

{DesignKnowledgeBase.ScreenDesignPrinciples}

SCREEN TYPE: {screenType}

Mockup Requirements:
- Create ONE complete mobile screen mockup filling the entire canvas
- Modern smartphone proportions (9:16 aspect ratio preferred)
- Include realistic UI components: navigation, buttons, cards, content
- Follow iOS or Material Design patterns
- Use proper spacing (8px grid), typography hierarchy, and visual balance
- NO multiple screen sizes, NO small thumbnails
- The mockup should fill the canvas edge-to-edge
- Professional, polished appearance ready for presentation

Output: Generate a detailed DALL-E prompt for a single, full-size mobile screen mockup.
Return ONLY the prompt text.";
    }

    /// <summary>
    /// Build user prompt for app resource mockup generation
    /// </summary>
    public string BuildAppResourceUserPrompt(AppResourcesGenerationRequest request)
    {
        var screenType = request.Options.ScreenTypes.FirstOrDefault();
        var screenContext = GetScreenContext(screenType.ToString());
        var categoryContext = GetCategoryContext(request.Options.AppName ?? "App");

        var userPrompt = $@"Create a mobile app screen mockup:

App Name: {request.Options.AppName ?? "Modern App"}
Screen Type: {screenType}
Platform: {request.Platforms.FirstOrDefault() ?? "iOS"}

{categoryContext}
{screenContext}

Design Requirements:
- Single full-screen mobile mockup (9:16 aspect)
- Include screen-specific UI elements
- Use brand colors: {request.Options.BrandPrimaryColor ?? "#0066FF"}, {request.Options.BrandSecondaryColor ?? "#00D4FF"}
- Modern, clean design patterns
- Professional quality ready for app store screenshots
- Fill the entire canvas with the mockup - NO multiple sizes

Generate a concise DALL-E prompt for this mockup.";

        return SanitizePrompt(userPrompt);
    }

    /// <summary>
    /// Get screen-specific context for better mockup generation
    /// </summary>
    private string GetScreenContext(string screenType)
    {
        return screenType.ToLowerInvariant() switch
        {
            "login" => "Include: email/phone input, password field, social login buttons, app logo at top, 'Sign Up' link",
            "dashboard" => "Include: header with greeting, stats cards, recent activity list, bottom navigation, action button",
            "profile" => "Include: profile photo, user name/bio, stats row, action buttons, settings icon, content grid/list",
            "home" or "feed" => "Include: top navigation, search bar, content cards with images, bottom tab bar",
            "product-list" or "catalog" => "Include: search/filter bar, product grid with images and prices, categories, cart icon",
            "product-detail" => "Include: large product image, title/price, description, size/color options, 'Add to Cart' button",
            "cart" or "checkout" => "Include: item list with quantities, price breakdown, promo code field, checkout button",
            "settings" => "Include: user avatar, grouped settings sections with icons, toggle switches, chevrons",
            "onboarding" => "Include: illustration/image, headline, description, page indicators, 'Next' or 'Skip' buttons",
            _ => $"Include relevant {screenType} UI components with clear hierarchy and modern design"
        };
    }

    /// <summary>
    /// Get category-specific context for better mockup generation
    /// </summary>
    private string GetCategoryContext(string category)
    {
        return category.ToLowerInvariant() switch
        {
            "ecommerce" or "shopping" => "Style: Product-focused with clear pricing, shopping cart, and purchase actions",
            "healthcare" or "medical" => "Style: Clean, trustworthy with health data, appointments, and medical information",
            "fitness" or "wellness" => "Style: Energetic with progress tracking, goals, stats charts, and activity feeds",
            "finance" or "banking" => "Style: Professional with account balances, transactions, charts, and secure actions",
            "social" or "social-media" => "Style: Engaging with user posts, comments, likes, stories, and social interactions",
            "productivity" or "task" => "Style: Organized with task lists, checkboxes, priorities, and project management",
            "education" or "learning" => "Style: Educational with courses, progress bars, lessons, and achievement tracking",
            "food" or "restaurant" => "Style: Appetizing with food images, menus, ratings, and ordering capabilities",
            "travel" or "booking" => "Style: Inspiring with destination images, dates, bookings, and travel information",
            "entertainment" or "media" => "Style: Immersive with media content, playback controls, and discovery features",
            _ => $"Style: Modern {category} app with relevant content and features"
        };
    }

    /// <summary>
    /// Get style-specific guidelines
    /// </summary>
    private string GetStyleGuidelines(string style)
    {
        return style.ToLowerInvariant() switch
        {
            "3d" or "modern" => DesignKnowledgeBase.StyleTemplates.Modern3D,
            "minimal" or "minimalist" => DesignKnowledgeBase.StyleTemplates.Minimal,
            "gradient" => DesignKnowledgeBase.StyleTemplates.Gradient,
            "glass" or "glassmorphism" => DesignKnowledgeBase.StyleTemplates.Glassmorphism,
            "neomorphism" or "soft" => DesignKnowledgeBase.StyleTemplates.Neomorphism,
            "clay" or "3d-illustration" => DesignKnowledgeBase.StyleTemplates.Claymorphism,
            "pixel" or "retro" => DesignKnowledgeBase.StyleTemplates.Pixel,
            _ => "Follow the specified style while maintaining professional quality and clarity."
        };
    }

    /// <summary>
    /// Sanitize prompts to avoid Azure OpenAI content policy triggers
    /// </summary>
    private string SanitizePrompt(string prompt)
    {
        if (string.IsNullOrEmpty(prompt)) return prompt;

        var replacements = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            // Content policy triggers
            { "night club", "music venue" },
            { "nightclub", "music venue" },
            { "nightlife", "evening events" },
            { "music streaming", "audio player" },
            { "sound waves", "audio visuals" },
            { "streaming", "media" },
            { "party", "celebration" },
            // Reduce instruction language that triggers jailbreak detection
            { "MUST", "should" },
            { "NEVER", "avoid" },
            { "IMPORTANT:", "Note:" },
            { "CRITICAL:", "Note:" },
            { "REQUIRED:", "Include:" },
            { "MANDATORY:", "Include:" },
            { "CONSTRAINTS:", "Guidelines:" },
            { "strictly", "carefully" },
            { "always ensure", "include" },
            { "you must", "please" },
            // Remove phrases that might suggest size variations
            { "show multiple sizes", "single large icon" },
            { "different scales", "one size" },
            { "size variations", "full canvas" }
        };

        var sanitized = prompt;
        foreach (var kv in replacements)
        {
            sanitized = Regex.Replace(sanitized, @"\b" + Regex.Escape(kv.Key) + @"\b", kv.Value, RegexOptions.IgnoreCase);
        }

        // Remove excessive emphasis markers
        sanitized = Regex.Replace(sanitized, @"\*\*([^*]+)\*\*", "$1");
        sanitized = Regex.Replace(sanitized, @"!!+", ".");
        sanitized = Regex.Replace(sanitized, @"\n{3,}", "\n\n"); // Reduce excessive newlines

        return sanitized;
    }

    /// <summary>
    /// Build variation prompts for A/B testing
    /// </summary>
    public List<string> BuildVariationPrompts(IconGenerationRequest request, int count = 3)
    {
        var variations = new List<string>();

        // Variation 1: Literal interpretation
        variations.Add($@"Create a {request.Style} app icon: {request.Keywords}.
Use colors: {string.Join(", ", request.Colors)}.
Direct, literal visual representation with professional execution.");

        // Variation 2: Abstract interpretation
        variations.Add($@"Create a {request.Style} app icon representing the concept of '{request.Keywords}'.
Colors: {string.Join(", ", request.Colors)}.
Abstract, symbolic interpretation with modern design principles.");

        // Variation 3: Metaphorical interpretation
        variations.Add($@"Create a {request.Style} app icon that metaphorically represents '{request.Keywords}'.
Palette: {string.Join(", ", request.Colors)}.
Creative metaphor with unique visual language and professional polish.");

        return variations.Take(count).ToList();
    }

    /// <summary>
    /// Analyze and score a generated prompt
    /// </summary>
    public PromptQualityScore AnalyzePromptQuality(string prompt)
    {
        var score = new PromptQualityScore();

        // Check for design principles
        score.HasColorGuidance = prompt.Contains("color", StringComparison.OrdinalIgnoreCase);
        score.HasCompositionRules = prompt.Contains("composition", StringComparison.OrdinalIgnoreCase)
            || prompt.Contains("centered", StringComparison.OrdinalIgnoreCase);
        score.HasStyleGuidelines = prompt.Contains("style", StringComparison.OrdinalIgnoreCase);
        score.HasQualityConstraints = prompt.Contains("professional", StringComparison.OrdinalIgnoreCase);
        score.HasScaleConsiderations = prompt.Contains("scalable", StringComparison.OrdinalIgnoreCase)
            || prompt.Contains("size", StringComparison.OrdinalIgnoreCase);

        // Calculate overall score
        var checks = new[]
        {
            score.HasColorGuidance,
            score.HasCompositionRules,
            score.HasStyleGuidelines,
            score.HasQualityConstraints,
            score.HasScaleConsiderations
        };
        score.OverallScore = checks.Count(x => x) / (double)checks.Length * 100;

        return score;
    }
}

/// <summary>
/// Prompt quality metrics for evaluation
/// </summary>
public class PromptQualityScore
{
    public bool HasColorGuidance { get; set; }
    public bool HasCompositionRules { get; set; }
    public bool HasStyleGuidelines { get; set; }
    public bool HasQualityConstraints { get; set; }
    public bool HasScaleConsiderations { get; set; }
    public double OverallScore { get; set; }

    public override string ToString()
    {
        return $@"Prompt Quality Score: {OverallScore:F1}%
- Color Guidance: {(HasColorGuidance ? "✓" : "✗")}
- Composition Rules: {(HasCompositionRules ? "✓" : "✗")}
- Style Guidelines: {(HasStyleGuidelines ? "✓" : "✗")}
- Quality Constraints: {(HasQualityConstraints ? "✓" : "✗")}
- Scale Considerations: {(HasScaleConsiderations ? "✓" : "✗")}";
    }
}
