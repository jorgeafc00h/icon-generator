using Xunit;
using Xunit.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using IconGenerator.Functions.Services;
using IconGenerator.Functions.Models;

namespace IconGenerator.Tests.Integration;

/// <summary>
/// Real image generation tests that create actual icons and save them to Azure Storage
/// These tests incur costs (~$0.04-0.08 per image)
/// </summary>
[Collection("Integration Tests")]
public class ImageGenerationTests
{
    private readonly TestFixture _fixture;
    private readonly ITestOutputHelper _output;
    private readonly AIService _aiService;
    private readonly IStorageService _storageService;
    private readonly IImageService _imageService;

    public ImageGenerationTests(TestFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _output = output;
        _aiService = _fixture.ServiceProvider.GetRequiredService<AIService>();
        _storageService = _fixture.ServiceProvider.GetRequiredService<IStorageService>();
        _imageService = _fixture.ServiceProvider.GetRequiredService<IImageService>();
    }

    [Fact(DisplayName = "Should generate icon and save to storage", Skip = "Costs money - remove Skip to run")]
    public async Task ShouldGenerateIconAndSaveToStorage()
    {
        // Arrange
        var request = new IconGenerationRequest
        {
            Keywords = "fitness tracker app",
            Style = "3D",
            Colors = new List<string> { "#FF5733", "#33FF57" },
            Quality = "standard"
        };
        var userId = "test-user";
        var iconId = Guid.NewGuid().ToString();

        _output.WriteLine("=== Real Image Generation Test ===");
        _output.WriteLine($"Request: {request.Keywords}");
        _output.WriteLine($"Style: {request.Style}");
        _output.WriteLine($"Colors: {string.Join(", ", request.Colors)}");
        _output.WriteLine($"Quality: {request.Quality}");
        _output.WriteLine("");

        // Act - Step 1: Enhance prompt
        _output.WriteLine("Step 1: Enhancing prompt with GPT-4o-mini...");
        var enhancedPrompt = await _aiService.EnhancePromptAsync(request);
        Assert.NotNull(enhancedPrompt);
        Assert.NotEmpty(enhancedPrompt);

        _output.WriteLine("Enhanced Prompt:");
        _output.WriteLine(enhancedPrompt);
        _output.WriteLine("");

        // Act - Step 2: Generate image with DALL-E 3
        _output.WriteLine("Step 2: Generating image with DALL-E 3...");
        var imageUrl = await _aiService.GenerateIconAsync(enhancedPrompt, request.Quality);
        Assert.NotNull(imageUrl);
        Assert.NotEmpty(imageUrl);

        _output.WriteLine($"Generated Image URL: {imageUrl}");
        _output.WriteLine("");

        // Act - Step 3: Upload to Azure Storage
        _output.WriteLine("Step 3: Uploading to Azure Storage...");
        var storedUrl = await _storageService.UploadImageAsync(imageUrl, userId, iconId);
        Assert.NotNull(storedUrl);
        Assert.NotEmpty(storedUrl);
        Assert.Contains("blob.core.windows.net", storedUrl);

        _output.WriteLine($"Stored URL: {storedUrl}");
        _output.WriteLine("");

        // Assert - Verify image is accessible
        _output.WriteLine("Step 4: Verifying image is accessible...");
        var imageData = await _storageService.DownloadImageAsync(storedUrl);
        Assert.NotNull(imageData);
        Assert.True(imageData.Length > 0);

        _output.WriteLine($"✅ Image downloaded successfully ({imageData.Length / 1024} KB)");
        _output.WriteLine("");
        _output.WriteLine("=== Test Complete ===");
        _output.WriteLine($"Icon ID: {iconId}");
        _output.WriteLine($"Storage URL: {storedUrl}");
        _output.WriteLine($"Cost: ~$0.04 (standard) or ~$0.08 (HD)");
    }

    [Fact(DisplayName = "Should generate multiple style variations", Skip = "Costs money - remove Skip to run")]
    public async Task ShouldGenerateMultipleStyleVariations()
    {
        var styles = new[] { "3D", "Minimal", "Gradient" };
        var concept = "weather app";
        var userId = "test-user";

        _output.WriteLine("=== Multiple Style Variations Test ===");
        _output.WriteLine($"Concept: {concept}");
        _output.WriteLine($"Styles: {string.Join(", ", styles)}");
        _output.WriteLine($"Total images: {styles.Length}");
        _output.WriteLine($"Estimated cost: ~${styles.Length * 0.04:F2}");
        _output.WriteLine("");

        var results = new List<(string style, string url)>();

        foreach (var style in styles)
        {
            _output.WriteLine($"--- Generating {style} style ---");

            var request = new IconGenerationRequest
            {
                Keywords = concept,
                Style = style,
                Colors = new List<string> { "#4A90E2", "#50C878" }
            };

            // Enhance prompt
            var enhancedPrompt = await _aiService.EnhancePromptAsync(request);
            _output.WriteLine($"Enhanced prompt length: {enhancedPrompt.Length} characters");

            // Generate image
            var imageUrl = await _aiService.GenerateIconAsync(enhancedPrompt);
            _output.WriteLine($"Generated: {imageUrl.Substring(0, Math.Min(80, imageUrl.Length))}...");

            // Upload to storage
            var iconId = Guid.NewGuid().ToString();
            var storedUrl = await _storageService.UploadImageAsync(imageUrl, userId, iconId);

            results.Add((style, storedUrl));
            _output.WriteLine($"Stored: {iconId}");
            _output.WriteLine("");

            // Rate limiting - avoid hitting quota
            await Task.Delay(2000);
        }

        // Assert all generated successfully
        Assert.Equal(styles.Length, results.Count);
        foreach (var result in results)
        {
            Assert.NotEmpty(result.url);
        }

        _output.WriteLine("=== All Variations Generated ===");
        foreach (var result in results)
        {
            _output.WriteLine($"{result.style}: {result.url}");
        }
    }

    [Fact(DisplayName = "Should generate and resize icons for platforms", Skip = "Costs money - remove Skip to run")]
    public async Task ShouldGenerateAndResizeForPlatforms()
    {
        // Arrange
        var request = new IconGenerationRequest
        {
            Keywords = "music player app",
            Style = "Gradient",
            Colors = new List<string> { "#FF6B9D", "#C06C84" }
        };
        var userId = "test-user";
        var iconId = Guid.NewGuid().ToString();

        _output.WriteLine("=== Platform Icon Generation Test ===");

        // Generate base icon
        var enhancedPrompt = await _aiService.EnhancePromptAsync(request);
        var imageUrl = await _aiService.GenerateIconAsync(enhancedPrompt);
        var storedUrl = await _storageService.UploadImageAsync(imageUrl, userId, iconId);

        _output.WriteLine($"Base icon generated: {iconId}");

        // Download for resizing
        var imageData = await _storageService.DownloadImageAsync(storedUrl);

        // Test iOS sizes
        var iosSizes = new[] { 20, 29, 40, 60, 76, 83, 1024 };
        _output.WriteLine($"\nGenerating {iosSizes.Length} iOS sizes...");

        foreach (var size in iosSizes)
        {
            var resized = await _imageService.ResizeImageAsync(imageData, size, size);
            Assert.NotNull(resized);
            Assert.True(resized.Length > 0);
            _output.WriteLine($"  ✓ {size}x{size} - {resized.Length / 1024}KB");
        }

        // Test Android adaptive icon (108dp)
        _output.WriteLine("\nGenerating Android adaptive icons...");
        var androidSizes = new[] { 108, 162, 216, 324, 432 }; // mdpi to xxxhdpi

        foreach (var size in androidSizes)
        {
            var foreground = await _imageService.CreateAdaptiveForegroundAsync(imageData, size, size);
            var background = await _imageService.CreateAdaptiveBackgroundAsync(size, size, "#FF6B9D");

            Assert.NotNull(foreground);
            Assert.NotNull(background);
            _output.WriteLine($"  ✓ {size}x{size} adaptive layers - FG: {foreground.Length / 1024}KB, BG: {background.Length / 1024}KB");
        }

        _output.WriteLine("\n✅ All platform sizes generated successfully!");
    }

    [Fact(DisplayName = "Should handle HD quality generation", Skip = "Costs money - remove Skip to run")]
    public async Task ShouldHandleHDQualityGeneration()
    {
        // Arrange
        var request = new IconGenerationRequest
        {
            Keywords = "productivity app with calendar and tasks",
            Style = "Modern 3D",
            Colors = new List<string> { "#667EEA", "#764BA2" },
            Quality = "hd"
        };
        var userId = "test-user";
        var iconId = Guid.NewGuid().ToString();

        _output.WriteLine("=== HD Quality Generation Test ===");
        _output.WriteLine("Note: HD costs 2x more (~$0.08 per image)");
        _output.WriteLine("");

        // Act
        var enhancedPrompt = await _aiService.EnhancePromptAsync(request);
        var imageUrl = await _aiService.GenerateIconAsync(enhancedPrompt, "hd");
        var storedUrl = await _storageService.UploadImageAsync(imageUrl, userId, iconId);

        // Assert
        var imageData = await _storageService.DownloadImageAsync(storedUrl);
        Assert.NotNull(imageData);

        _output.WriteLine($"HD Image Size: {imageData.Length / 1024} KB");
        _output.WriteLine($"Storage URL: {storedUrl}");
        _output.WriteLine("✅ HD quality image generated successfully");
    }

    [Fact(DisplayName = "Should generate app mockup style icon")]
    public async Task ShouldGenerateAppMockupStyleIcon()
    {
        // This will be used for Phase 2 - app mockup generation
        // For now, just testing the concept

        var request = new IconGenerationRequest
        {
            Keywords = "modern banking app login screen, card-based UI, clean interface",
            Style = "UI Mockup",
            Colors = new List<string> { "#1E3A8A", "#3B82F6", "#FFFFFF" }
        };

        var enhancedPrompt = await _aiService.EnhancePromptAsync(request);

        Assert.NotNull(enhancedPrompt);
        Assert.Contains("interface", enhancedPrompt, StringComparison.OrdinalIgnoreCase);

        _output.WriteLine("=== App Mockup Prompt Test ===");
        _output.WriteLine("Enhanced Prompt:");
        _output.WriteLine(enhancedPrompt);
        _output.WriteLine("");
        _output.WriteLine("Note: Actual mockup generation will be implemented in Phase 2");
    }
}
