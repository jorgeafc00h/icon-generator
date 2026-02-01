using Xunit;
using Xunit.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using IconGenerator.Functions.Services;
using IconGenerator.Functions.Models;

namespace IconGenerator.Tests.Integration;

/// <summary>
/// Integration tests for Azure OpenAI prompt enhancement
/// Tests GPT-4o-mini deployment without generating images (cost optimization)
/// </summary>
[Collection("Integration Tests")]
public class PromptEnhancementTests
{
    private readonly TestFixture _fixture;
    private readonly ITestOutputHelper _output;
    private readonly AIService _aiService;
    private readonly PromptEngineeringService _promptService;

    public PromptEnhancementTests(TestFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _output = output;
        _aiService = _fixture.ServiceProvider.GetRequiredService<AIService>();
        _promptService = _fixture.ServiceProvider.GetRequiredService<PromptEngineeringService>();
    }

    [Fact(DisplayName = "Should enhance basic prompt with design guidelines")]
    public async Task ShouldEnhanceBasicPrompt()
    {
        // Arrange
        var request = new IconGenerationRequest
        {
            Keywords = "fitness tracker",
            Style = "3D",
            Colors = new List<string> { "#FF5733", "#33FF57" }
        };

        // Act
        var enhanced = await _aiService.EnhancePromptAsync(request);

        // Assert
        Assert.NotNull(enhanced);
        Assert.NotEmpty(enhanced);
        Assert.Contains("app icon", enhanced, StringComparison.OrdinalIgnoreCase);

        _output.WriteLine("=== Enhanced Prompt ===");
        _output.WriteLine(enhanced);
        _output.WriteLine("");
    }

    [Theory(DisplayName = "Should enhance prompts across different styles")]
    [InlineData("3D", "music player")]
    [InlineData("Minimal", "weather app")]
    [InlineData("Gradient", "calendar")]
    [InlineData("Glassmorphism", "messaging")]
    public async Task ShouldEnhancePromptsAcrossDifferentStyles(string style, string concept)
    {
        // Arrange
        var request = new IconGenerationRequest
        {
            Keywords = concept,
            Style = style,
            Colors = new List<string> { "#4A90E2", "#50C878" }
        };

        // Act
        var enhanced = await _aiService.EnhancePromptAsync(request);

        // Assert
        Assert.NotNull(enhanced);
        Assert.NotEmpty(enhanced);
        Assert.Contains(style.ToLower(), enhanced, StringComparison.OrdinalIgnoreCase);

        _output.WriteLine($"=== {style} Style - {concept} ===");
        _output.WriteLine(enhanced);
        _output.WriteLine("");
    }

    [Fact(DisplayName = "Should include color guidance in enhanced prompt")]
    public async Task ShouldIncludeColorGuidance()
    {
        // Arrange
        var request = new IconGenerationRequest
        {
            Keywords = "social media app",
            Style = "Gradient",
            Colors = new List<string> { "#FF6B6B", "#4ECDC4", "#45B7D1" }
        };

        // Act
        var enhanced = await _aiService.EnhancePromptAsync(request);

        // Assert
        Assert.NotNull(enhanced);
        var hasColorMention = request.Colors.Any(c => enhanced.Contains(c, StringComparison.OrdinalIgnoreCase)) ||
                             enhanced.Contains("color", StringComparison.OrdinalIgnoreCase);
        Assert.True(hasColorMention, "Enhanced prompt should include color guidance");

        _output.WriteLine("=== Color Guidance Test ===");
        _output.WriteLine($"Colors: {string.Join(", ", request.Colors)}");
        _output.WriteLine(enhanced);
        _output.WriteLine("");
    }

    [Fact(DisplayName = "Should build system prompt with design knowledge")]
    public void ShouldBuildSystemPromptWithDesignKnowledge()
    {
        // Arrange
        var style = "Minimal";

        // Act
        var systemPrompt = _promptService.BuildIconSystemPrompt(style);

        // Assert
        Assert.NotNull(systemPrompt);
        Assert.NotEmpty(systemPrompt);
        Assert.Contains("designer", systemPrompt, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(style, systemPrompt, StringComparison.OrdinalIgnoreCase);

        _output.WriteLine("=== System Prompt ===");
        _output.WriteLine(systemPrompt);
        _output.WriteLine("");
    }

    [Fact(DisplayName = "Should analyze prompt quality score")]
    public void ShouldAnalyzePromptQualityScore()
    {
        // Arrange
        var goodPrompt = "Create a modern, minimalist app icon for a fitness tracker featuring a stylized heart rate monitor with clean lines, vibrant green and blue gradient colors, and a professional rounded square shape optimized for iOS and Android platforms.";
        var poorPrompt = "fitness icon";

        // Act
        var goodScore = _promptService.AnalyzePromptQuality(goodPrompt);
        var poorScore = _promptService.AnalyzePromptQuality(poorPrompt);

        // Assert
        Assert.True(goodScore.OverallScore > poorScore.OverallScore, "Detailed prompt should score higher than minimal prompt");
        Assert.InRange(goodScore.OverallScore, 0, 100);
        Assert.InRange(poorScore.OverallScore, 0, 100);

        _output.WriteLine("=== Quality Scores ===");
        _output.WriteLine($"Good Prompt Score: {goodScore.OverallScore:F1}%");
        _output.WriteLine($"Poor Prompt Score: {poorScore.OverallScore:F1}%");
        _output.WriteLine("");
    }

    [Fact(DisplayName = "Should create prompt variations for A/B testing")]
    public void ShouldCreatePromptVariations()
    {
        // Arrange
        var request = new IconGenerationRequest
        {
            Keywords = "fitness tracker app icon",
            Style = "3D",
            Colors = new List<string> { "#FF5733", "#33FF57" }
        };

        // Act
        var variations = _promptService.BuildVariationPrompts(request, 3);

        // Assert
        Assert.NotNull(variations);
        Assert.Equal(3, variations.Count);
        Assert.NotEqual(variations[0], variations[1]);
        Assert.NotEqual(variations[0], variations[2]);

        _output.WriteLine("=== Prompt Variations ===");
        _output.WriteLine("Variation 1:");
        _output.WriteLine(variations[0]);
        _output.WriteLine("\nVariation 2:");
        _output.WriteLine(variations[1]);
        _output.WriteLine("\nVariation 3:");
        _output.WriteLine(variations[2]);
        _output.WriteLine("");
    }
}
