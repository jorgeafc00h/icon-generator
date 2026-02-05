using Xunit;
using Xunit.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using IconGenerator.Functions.Services;
using IconGenerator.Functions.Models;

namespace IconGenerator.Tests.Integration;

/// <summary>
/// Icon Style Samples Generation Tests
/// Generates 2 sample icons for each style category to display on the web app
/// These are REAL icons generated with DALL-E 3 and saved to Azure Storage
///
/// IMPORTANT: Remove [Skip] attribute to generate actual samples
/// This will cost money (~$0.04 per icon × 36 icons = ~$1.44 total)
/// </summary>
[Collection("Integration Tests")]
public class IconStyleSamplesGenerationTests
{
    private readonly TestFixture _fixture;
    private readonly ITestOutputHelper _output;
    private readonly AIService _aiService;
    private readonly IStorageService _storageService;

    // Test user for sample generation
    private const string SAMPLE_USER_ID = "samples";

    public IconStyleSamplesGenerationTests(TestFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _output = output;
        _aiService = _fixture.ServiceProvider.GetRequiredService<AIService>();
        _storageService = _fixture.ServiceProvider.GetRequiredService<IStorageService>();
    }

    /// <summary>
    /// Style definitions with sample prompts
    /// Each style gets 2 different icon concepts to showcase variety
    /// </summary>
    private readonly Dictionary<string, List<(string concept, string keywords)>> _stylesSamples = new()
    {
        {
            "3D", new List<(string, string)>
            {
                ("Music Player", "audio player app with headphones and equalizer"),
                ("Fitness Tracker", "fitness and health tracking app with heart rate monitor")
            }
        },
        {
            "Minimal", new List<(string, string)>
            {
                ("Task Manager", "productivity and task management app"),
                ("Weather App", "weather forecast app with simple icons")
            }
        },
        {
            "Gradient", new List<(string, string)>
            {
                ("Photo Editor", "photo editing and filters app"),
                ("Social Network", "social media networking app")
            }
        },
        {
            "Glassmorphism", new List<(string, string)>
            {
                ("Calendar", "calendar and scheduling app"),
                ("Messaging", "instant messaging and chat app")
            }
        },
        {
            "Neomorphism", new List<(string, string)>
            {
                ("Banking", "mobile banking and finance app"),
                ("Smart Home", "home automation and IoT control app")
            }
        },
        {
            "Clay", new List<(string, string)>
            {
                ("Cooking", "recipe and cooking app"),
                ("Travel", "travel planning and booking app")
            }
        },
        {
            "Pixel", new List<(string, string)>
            {
                ("Gaming", "retro gaming platform app"),
                ("Music Creator", "8-bit music creation app")
            }
        },
        {
            "Flat", new List<(string, string)>
            {
                ("E-Learning", "online education and courses app"),
                ("Shopping", "e-commerce shopping app")
            }
        },
        {
            "Isometric", new List<(string, string)>
            {
                ("City Builder", "city building and management game"),
                ("Delivery", "package delivery tracking app")
            }
        },
        {
            "Hand-drawn", new List<(string, string)>
            {
                ("Journal", "personal diary and journaling app"),
                ("Notes", "handwritten notes and sketching app")
            }
        },
        {
            "Geometric", new List<(string, string)>
            {
                ("Analytics", "data analytics and charts app"),
                ("Crypto", "cryptocurrency trading app")
            }
        },
        {
            "Abstract", new List<(string, string)>
            {
                ("Meditation", "mindfulness and meditation app"),
                ("Art Gallery", "digital art gallery app")
            }
        },
        {
            "Retro", new List<(string, string)>
            {
                ("Arcade", "vintage arcade games app"),
                ("Radio", "retro radio and podcast app")
            }
        },
        {
            "Neon", new List<(string, string)>
            {
                ("Music Festival", "live music and concert event app"),
                ("Racing Game", "neon-style racing game app")
            }
        },
        {
            "Watercolor", new List<(string, string)>
            {
                ("Painting", "digital painting and art app"),
                ("Garden", "gardening and plant care app")
            }
        },
        {
            "Metallic", new List<(string, string)>
            {
                ("Luxury", "luxury lifestyle and premium app"),
                ("Tools", "professional tools and utilities app")
            }
        },
        {
            "Cartoon", new List<(string, string)>
            {
                ("Kids Learning", "children's educational app"),
                ("Pet Care", "pet adoption and care app")
            }
        },
        {
            "Realistic", new List<(string, string)>
            {
                ("Camera", "professional camera and photography app"),
                ("Real Estate", "property viewing and real estate app")
            }
        }
    };

    [Fact(DisplayName = "Should generate 2 sample icons for each style (36 total)"
    ,Skip = "Costs money - remove Skip to generate samples"
     )]
    public async Task ShouldGenerateAllStyleSamples()
    {
        _output.WriteLine("╔════════════════════════════════════════════════════════════════╗");
        
        _output.WriteLine("╔════════════════════════════════════════════════════════════════╗");
        _output.WriteLine("");

        var totalStyles = _stylesSamples.Count;
        var totalIcons = totalStyles * 2;
        var estimatedCost = totalIcons * 0.04m; // $0.04 per standard quality icon

        _output.WriteLine($"📊 Generation Plan:");
        _output.WriteLine($"   • Styles to process: {totalStyles}");
        _output.WriteLine($"   • Icons per style: 2");
        _output.WriteLine($"   • Total icons: {totalIcons}");
        _output.WriteLine($"   • Estimated cost: ${estimatedCost:F2}");
        _output.WriteLine("");

        var generatedSamples = new List<GeneratedSample>();
        var currentStyle = 1;

        foreach (var (style, samples) in _stylesSamples)
        {
            _output.WriteLine($"━━━ [{currentStyle}/{totalStyles}] Processing Style: {style} ━━━");
            _output.WriteLine("");

            var sampleNumber = 1;
            foreach (var (concept, keywords) in samples)
            {
                _output.WriteLine($"   🎨 Sample {sampleNumber}: {concept}");
                var enhancedPrompt= "";
                try
                {
                    // Create icon request
                    var request = new IconGenerationRequest
                    {
                        Keywords = keywords,
                        Style = style,
                        Colors = GetStyleColors(style),
                        Quality = "standard"
                    };

                    // Enhance prompt
                    enhancedPrompt = await _aiService.EnhancePromptAsync(request);
                    _output.WriteLine($"      ✓ Prompt enhanced ({enhancedPrompt.Length} chars)");

                    // Generate icon with DALL-E 3
                    var imageUrl = await _aiService.GenerateIconAsync(enhancedPrompt, request.Quality);
                    _output.WriteLine($"      ✓ Icon generated: {imageUrl.Substring(0, Math.Min(50, imageUrl.Length))}...");

                    // Save to Azure Storage
                    var iconId = $"{style.ToLower()}-{concept.ToLower().Replace(" ", "-")}-{Guid.NewGuid().ToString().Substring(0, 8)}";
                    var storedUrl = await _storageService.UploadImageAsync(imageUrl, SAMPLE_USER_ID, iconId);
                    _output.WriteLine($"      ✓ Saved to storage: {iconId}");

                    // Verify download works
                    var imageData = await _storageService.DownloadImageAsync(storedUrl);
                    Assert.True(imageData.Length > 0, "Downloaded image should have content");
                    _output.WriteLine($"      ✓ Verified ({imageData.Length / 1024} KB)");

                    generatedSamples.Add(new GeneratedSample
                    {
                        Style = style,
                        Concept = concept,
                        IconId = iconId,
                        StorageUrl = storedUrl,
                        EnhancedPrompt = enhancedPrompt,
                        FileSizeKB = imageData.Length / 1024
                    });

                    _output.WriteLine($"      ✅ Success!");
                    _output.WriteLine("");
                }
                catch (Exception ex)
                {
                    _output.WriteLine($"      ❌ Error: {ex.Message}");
                    _output.WriteLine("");
                }

                sampleNumber++;
            }

            currentStyle++;
            _output.WriteLine("");
        }

        // Summary
        _output.WriteLine("╔════════════════════════════════════════════════════════════════╗");
        _output.WriteLine("║                    GENERATION SUMMARY                          ║");
        _output.WriteLine("╔════════════════════════════════════════════════════════════════╗");
        _output.WriteLine("");
        _output.WriteLine($"📈 Results:");
        _output.WriteLine($"   • Successfully generated: {generatedSamples.Count}/{totalIcons}");
        _output.WriteLine($"   • Total size: {generatedSamples.Sum(s => s.FileSizeKB)} KB");
        _output.WriteLine($"   • Actual cost: ${generatedSamples.Count * 0.04m:F2}");
        _output.WriteLine("");

        // Group by style
        _output.WriteLine("📊 By Style:");
        foreach (var styleGroup in generatedSamples.GroupBy(s => s.Style))
        {
            _output.WriteLine($"   • {styleGroup.Key,-15} : {styleGroup.Count()} icons ({styleGroup.Sum(s => s.FileSizeKB)} KB)");
        }
        _output.WriteLine("");

        // Export URLs for web app (JSON format)
        _output.WriteLine("📦 Sample URLs (JSON for web app):");
        _output.WriteLine("[");
        foreach (var sample in generatedSamples)
        {
            _output.WriteLine($"  {{");
            _output.WriteLine($"    \"style\": \"{sample.Style}\",");
            _output.WriteLine($"    \"concept\": \"{sample.Concept}\",");
            _output.WriteLine($"    \"iconId\": \"{sample.IconId}\",");
            _output.WriteLine($"    \"url\": \"{sample.StorageUrl}\",");
            _output.WriteLine($"    \"sizeKB\": {sample.FileSizeKB}");
            _output.WriteLine($"  }}{(sample != generatedSamples.Last() ? "," : "")}");
        }
        _output.WriteLine("]");
        _output.WriteLine("");

        Assert.True(generatedSamples.Count >= totalIcons * 0.8,
            $"At least 80% of samples should be generated successfully. Got {generatedSamples.Count}/{totalIcons}");
    }

    [Theory(DisplayName = "Should generate sample icon for specific style"
    //, Skip = "Costs money - remove Skip to generate"
    )]
    [InlineData("3D", "Music Player", "audio player app with headphones and equalizer")]
    [InlineData("Minimal", "Weather App", "weather forecast app with simple icons")]
    [InlineData("Gradient", "Social Network", "social media networking app")]
    [InlineData("Isometric", "City Builder", "city building and management game")]
    [InlineData("Neon", "Music Festival", "live music and concert event app")]
    public async Task ShouldGenerateSingleStyleSample(string style, string concept, string keywords)
    {
        _output.WriteLine($"🎨 Generating {style} style sample: {concept}");
        _output.WriteLine("");

        // Create request
        var request = new IconGenerationRequest
        {
            Keywords = keywords,
            Style = style,
            Colors = GetStyleColors(style),
            Quality = "standard"
        };

        // Enhance prompt
        var enhancedPrompt = await _aiService.EnhancePromptAsync(request);
        _output.WriteLine($"✓ Enhanced Prompt:");
        _output.WriteLine($"  {enhancedPrompt.Substring(0, Math.Min(200, enhancedPrompt.Length))}...");
        _output.WriteLine("");

        Assert.NotNull(enhancedPrompt);
        Assert.Contains(style.ToLower(), enhancedPrompt.ToLower());
        Assert.True(enhancedPrompt.Length > 100, "Enhanced prompt should be substantial");

        // Generate icon
        var imageUrl = await _aiService.GenerateIconAsync(enhancedPrompt, request.Quality);
        _output.WriteLine($"✓ Generated Icon URL:");
        _output.WriteLine($"  {imageUrl}");
        _output.WriteLine("");

        Assert.NotNull(imageUrl);
        Assert.StartsWith("http", imageUrl);

        // Save to storage
        var iconId = $"{style.ToLower()}-{concept.ToLower().Replace(" ", "-")}-sample";
        var storedUrl = await _storageService.UploadImageAsync(imageUrl, SAMPLE_USER_ID, iconId);
        _output.WriteLine($"✓ Saved to Azure Storage:");
        _output.WriteLine($"  ID: {iconId}");
        _output.WriteLine($"  URL: {storedUrl}");
        _output.WriteLine("");

        Assert.NotNull(storedUrl);
        Assert.Contains(iconId, storedUrl);

        // Verify
        var imageData = await _storageService.DownloadImageAsync(storedUrl);
        _output.WriteLine($"✓ Verified Download:");
        _output.WriteLine($"  Size: {imageData.Length / 1024} KB");
        _output.WriteLine($"  Format: PNG");
        _output.WriteLine("");

        Assert.True(imageData.Length > 0);
        Assert.True(imageData.Length > 10000, "Image should be at least 10KB");

        _output.WriteLine("✅ Sample generated successfully!");
    }

    [Fact(DisplayName = "Should validate all style names are correct")]
    public void ShouldValidateStyleNames()
    {
        var validStyles = new[]
        {
            "3D", "Minimal", "Gradient", "Glassmorphism", "Neomorphism",
            "Clay", "Pixel", "Flat", "Isometric", "Hand-drawn",
            "Geometric", "Abstract", "Retro", "Neon", "Watercolor",
            "Metallic", "Cartoon", "Realistic"
        };

        _output.WriteLine("📋 Validating style definitions...");
        _output.WriteLine("");

        foreach (var (style, _) in _stylesSamples)
        {
            Assert.Contains(style, validStyles);
            _output.WriteLine($"   ✓ {style}");
        }

        _output.WriteLine("");
        _output.WriteLine($"✅ All {_stylesSamples.Count} styles are valid");
    }

    [Fact(DisplayName = "Should calculate total generation cost")]
    public void ShouldCalculateTotalCost()
    {
        var totalIcons = _stylesSamples.Count * 2; // 2 samples per style
        var costPerIcon = 0.04m; // Standard quality
        var totalCost = totalIcons * costPerIcon;

        _output.WriteLine("💰 Cost Analysis:");
        _output.WriteLine("");
        _output.WriteLine($"   • Total styles: {_stylesSamples.Count}");
        _output.WriteLine($"   • Samples per style: 2");
        _output.WriteLine($"   • Total icons to generate: {totalIcons}");
        _output.WriteLine($"   • Cost per icon (standard): ${costPerIcon:F2}");
        _output.WriteLine($"   • Total cost: ${totalCost:F2}");
        _output.WriteLine("");
        _output.WriteLine("💡 ROI:");
        _output.WriteLine($"   • Traditional design cost (36 icons × $50): ${totalIcons * 50:F2}");
        _output.WriteLine($"   • AI generation cost: ${totalCost:F2}");
        _output.WriteLine($"   • Savings: ${(totalIcons * 50) - totalCost:F2} ({((1 - (totalCost / (totalIcons * 50))) * 100):F1}%)");

        Assert.Equal(36, totalIcons);
        Assert.Equal(1.44m, totalCost);
    }

    // Helper: Get appropriate colors for each style
    private List<string> GetStyleColors(string style)
    {
        return style switch
        {
            "3D" => new List<string> { "#FF6B6B", "#4ECDC4", "#45B7D1" }, // Coral, Teal, Blue
            "Minimal" => new List<string> { "#2C3E50", "#ECF0F1" }, // Dark, Light
            "Gradient" => new List<string> { "#667EEA", "#764BA2", "#F093FB" }, // Purple gradient
            "Glassmorphism" => new List<string> { "#FFFFFF", "#4A90E2", "#A0D8F1" }, // Glass effect
            "Neomorphism" => new List<string> { "#E0E5EC", "#A3B9CC" }, // Soft gray
            "Clay" => new List<string> { "#FFB6C1", "#FFD700", "#98D8C8" }, // Pastel
            "Pixel" => new List<string> { "#FF0000", "#00FF00", "#0000FF" }, // Primary RGB
            "Flat" => new List<string> { "#3498DB", "#E74C3C", "#F39C12" }, // Material
            "Isometric" => new List<string> { "#5C7CFA", "#51CF66", "#FFD43B" }, // Vibrant
            "Hand-drawn" => new List<string> { "#8B4513", "#DEB887", "#F5DEB3" }, // Sketch tones
            "Geometric" => new List<string> { "#6C5CE7", "#00B894", "#FDCB6E" }, // Modern
            "Abstract" => new List<string> { "#A29BFE", "#FD79A8", "#FDCB6E" }, // Artistic
            "Retro" => new List<string> { "#FF6B9D", "#FFA07A", "#87CEEB" }, // Vintage
            "Neon" => new List<string> { "#FF00FF", "#00FFFF", "#FFFF00" }, // Neon bright
            "Watercolor" => new List<string> { "#FFB6C1", "#E6E6FA", "#B0E0E6" }, // Soft pastels
            "Metallic" => new List<string> { "#C0C0C0", "#FFD700", "#B87333" }, // Metals
            "Cartoon" => new List<string> { "#FF69B4", "#FFA500", "#00CED1" }, // Playful
            "Realistic" => new List<string> { "#8B7355", "#4A4A4A", "#D3D3D3" }, // Natural
            _ => new List<string> { "#4A90E2", "#50C878" } // Default
        };
    }

    // Helper class for tracking generated samples
    private class GeneratedSample
    {
        public string Style { get; set; } = string.Empty;
        public string Concept { get; set; } = string.Empty;
        public string IconId { get; set; } = string.Empty;
        public string StorageUrl { get; set; } = string.Empty;
        public string EnhancedPrompt { get; set; } = string.Empty;
        public long FileSizeKB { get; set; }
    }
}
