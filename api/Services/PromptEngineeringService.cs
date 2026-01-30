namespace IconGenerator.Functions.Services;

using IconGenerator.Functions.Models;
using IconGenerator.Functions.Prompts;
using Microsoft.Extensions.Logging;

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

        var systemPrompt = $@"You are an elite UI/UX designer and icon design specialist with expertise in creating professional app icons for iOS, Android, and web platforms.

{DesignKnowledgeBase.IconDesignPrinciples}

{styleGuidelines}

{DesignKnowledgeBase.CompositionRules}

IMPORTANT CONSTRAINTS:
- NO TEXT, LETTERS, OR WORDS in the icon
- NO complex scenes with multiple subjects
- NO photorealistic faces or people
- MUST work at all sizes (29px to 1024px)
- MUST have clear silhouette
- MUST follow platform guidelines

OUTPUT FORMAT:
Generate a detailed DALL-E prompt that incorporates all these design principles.
Focus on composition, color harmony, visual hierarchy, and professional execution.
Return ONLY the prompt text, no explanations.";

        return systemPrompt;
    }

    /// <summary>
    /// Build user prompt with enhanced context
    /// </summary>
    public string BuildIconUserPrompt(IconGenerationRequest request)
    {
        var colorGuidance = DesignKnowledgeBase.ColorPalettes.GetPalettePrompt(request.Colors);

        var userPrompt = $@"Create a professional {request.Style} app icon for: {request.Keywords}

COLOR PALETTE:
{colorGuidance}

STYLE: {request.Style}

REQUIREMENTS:
- Icon should be centered on a clean background
- Design must be scalable and recognizable
- Follow {request.Style} style guidelines strictly
- Ensure professional quality suitable for app stores
- Create unique, memorable design avoiding clichés
- Optimize for {request.Quality} quality rendering

Generate the complete DALL-E 3 prompt now.";

        return userPrompt;
    }

    /// <summary>
    /// Build system prompt for UI screen generation
    /// </summary>
    public string BuildScreenSystemPrompt(string screenType)
    {
        return $@"You are an elite UI/UX designer specializing in modern mobile and web application interfaces.

{DesignKnowledgeBase.ScreenDesignPrinciples}

{DesignKnowledgeBase.CompositionRules}

SCREEN TYPE: {screenType}
(e.g., login, dashboard, profile, onboarding, etc.)

IMPORTANT GUIDELINES:
- Use modern UI patterns (cards, bottom sheets, floating buttons)
- Follow platform conventions (iOS or Android Material Design)
- Ensure proper spacing and typography
- Create clear visual hierarchy
- Include realistic UI elements (buttons, inputs, navigation)
- Use appropriate components for the screen type

OUTPUT FORMAT:
Generate a detailed DALL-E prompt for a complete UI screen design.
Include specific UI components, layout structure, and visual styling.
Return ONLY the prompt text.";
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
