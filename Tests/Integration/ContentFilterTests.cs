using IconGenerator.Functions.Models;
using IconGenerator.Functions.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace IconGenerator.Tests.Integration;

/// <summary>
/// Test to verify prompt sanitization prevents content filter violations
/// </summary>
public class ContentFilterTests
{
    [Fact]
    public void BuildIconUserPrompt_ProducesShortNaturalLanguagePrompt()
    {
        // Arrange
        var logger = NullLogger<PromptEngineeringService>.Instance;
        var service = new PromptEngineeringService(logger);
        
        var request = new IconGenerationRequest
        {
            Keywords = "fitness health tracking heart rate monitor",
            Style = "3D",
            Colors = new List<string> { "#FF6B6B", "#4ECDC4", "#45B7D1" },
            Quality = "hd"
        };

        // Act
        var userPrompt = service.BuildIconUserPrompt(request);

        // Assert
        Assert.NotEmpty(userPrompt);
        
        // Verify no instruction-style language
        Assert.DoesNotContain("MUST", userPrompt);
        Assert.DoesNotContain("REQUIRED", userPrompt);
        Assert.DoesNotContain("CRITICAL", userPrompt);
        Assert.DoesNotContain("IMPORTANT", userPrompt);
        Assert.DoesNotContain("strictly", userPrompt, StringComparison.OrdinalIgnoreCase);
        
        // Verify natural language
        Assert.Contains("Design", userPrompt);
        Assert.Contains("icon", userPrompt);
        
        // Verify it's concise (aim for under 500 chars in user prompt)
        Assert.True(userPrompt.Length < 500, 
            $"User prompt too long: {userPrompt.Length} chars. Content: {userPrompt}");
    }

    [Fact]
    public void BuildIconSystemPrompt_ProducesSimplifiedGuidelines()
    {
        // Arrange
        var logger = NullLogger<PromptEngineeringService>.Instance;
        var service = new PromptEngineeringService(logger);

        // Act
        var systemPrompt = service.BuildIconSystemPrompt("3D");

        // Assert
        Assert.NotEmpty(systemPrompt);
        
        // Verify no excessive all-caps sections
        var allCapsMatches = System.Text.RegularExpressions.Regex.Matches(
            systemPrompt, 
            @"\b[A-Z]{4,}\b");
        Assert.True(allCapsMatches.Count < 5, 
            "Too many all-caps words that look like directives");
        
        // Verify simplified structure
        Assert.DoesNotContain("CONSTRAINTS:", systemPrompt);
        Assert.DoesNotContain("REQUIREMENTS:", systemPrompt);
        
        // Should still have guidelines
        Assert.Contains("guidelines", systemPrompt, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void BuildIconUserPrompt_TravelApp_NoContentViolation()
    {
        // Arrange - Test case from the actual error
        var logger = NullLogger<PromptEngineeringService>.Instance;
        var service = new PromptEngineeringService(logger);
        
        var request = new IconGenerationRequest
        {
            Keywords = "travel planning and booking app featuring globe suitcase airplane",
            Style = "Clay",
            Colors = new List<string> { "#FFB6C1", "#FFD700", "#98D8C8" },
            Quality = "standard"
        };

        // Act
        var userPrompt = service.BuildIconUserPrompt(request);

        // Assert
        Assert.NotEmpty(userPrompt);
        
        // Check for patterns that trigger content filters
        Assert.DoesNotContain("rejected", userPrompt, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("safety system", userPrompt, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("not allowed", userPrompt, StringComparison.OrdinalIgnoreCase);
        
        // Verify natural phrasing
        Assert.Contains("design", userPrompt, StringComparison.OrdinalIgnoreCase);
        
        Console.WriteLine($"Generated prompt ({userPrompt.Length} chars):\n{userPrompt}");
    }

    [Fact]
    public void SanitizePrompt_RemovesProblematicPatterns()
    {
        // Arrange
        var logger = NullLogger<PromptEngineeringService>.Instance;
        var service = new PromptEngineeringService(logger);
        
        // Create a request with potentially problematic content
        var request = new IconGenerationRequest
        {
            Keywords = "nightclub streaming music with sound waves",
            Style = "Modern",
            Colors = new List<string> { "#FF0000" },
            Quality = "standard"
        };

        // Act
        var prompt = service.BuildIconUserPrompt(request);

        // Assert
        // Verify sanitization happened
        Assert.DoesNotContain("nightclub", prompt, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("streaming", prompt, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("sound waves", prompt, StringComparison.OrdinalIgnoreCase);
        
        // Should be replaced with safe alternatives
        Assert.Contains("music venue", prompt, StringComparison.OrdinalIgnoreCase);
        
        Console.WriteLine($"Sanitized prompt:\n{prompt}");
    }
}
