using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using IconGenerator.Tests;
using IconGenerator.Functions.Services;

// Load environment from .env file
DotNetEnv.Env.Load();

Console.WriteLine("===========================================");
Console.WriteLine("  Icon Generator - Integration Tests");
Console.WriteLine("===========================================\n");

// Setup test fixture
using var fixture = new TestFixture();

var aiService = fixture.ServiceProvider.GetRequiredService<AIService>();
var promptService = fixture.ServiceProvider.GetRequiredService<PromptEngineeringService>();

Console.WriteLine("✅ Test environment initialized");
Console.WriteLine($"✅ Azure OpenAI Endpoint: {fixture.Configuration["AZURE_OPENAI_ENDPOINT"]}");
Console.WriteLine($"✅ GPT-4o-mini Deployment: {fixture.Configuration["GPT4O_MINI_DEPLOYMENT_NAME"]}\n");

Console.WriteLine("Select test to run:");
Console.WriteLine("1. Basic Prompt Enhancement");
Console.WriteLine("2. Style Variations (3D, Minimal, Gradient)");
Console.WriteLine("3. Color Palette Test");
Console.WriteLine("4. Quality Score Evaluation");
Console.WriteLine("5. Prompt A/B Testing Variations");
Console.WriteLine("6. Run All Tests (via xUnit)");
Console.WriteLine("0. Exit\n");

Console.Write("Enter choice: ");
var choice = Console.ReadLine();

try
{
    switch (choice)
    {
        case "1":
            await TestBasicEnhancement(aiService);
            break;
        case "2":
            await TestStyleVariations(aiService);
            break;
        case "3":
            await TestColorPalette(aiService);
            break;
        case "4":
            TestQualityScores(promptService);
            break;
        case "5":
            TestPromptVariations(promptService);
            break;
        case "6":
            Console.WriteLine("\nTo run all xUnit tests, use:");
            Console.WriteLine("  dotnet test\n");
            break;
        case "0":
            Console.WriteLine("Exiting...");
            return;
        default:
            Console.WriteLine("Invalid choice");
            break;
    }
}
catch (Exception ex)
{
    Console.WriteLine($"\n❌ Error: {ex.Message}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"   Inner: {ex.InnerException.Message}");
    }
}

Console.WriteLine("\n✅ Test completed!");

// Test implementations
static async Task TestBasicEnhancement(AIService aiService)
{
    Console.WriteLine("\n=== Basic Prompt Enhancement ===\n");

    var request = new IconGenerator.Functions.Models.IconGenerationRequest
    {
        Keywords = "fitness tracker",
        Style = "3D",
        Colors = new List<string> { "#FF5733", "#33FF57" }
    };

    Console.WriteLine($"Input: {request.Keywords}");
    Console.WriteLine($"Style: {request.Style}");
    Console.WriteLine($"Colors: {string.Join(", ", request.Colors)}\n");

    Console.WriteLine("Enhancing prompt...\n");
    var enhanced = await aiService.EnhancePromptAsync(request);

    Console.WriteLine("Enhanced Prompt:");
    Console.WriteLine("─────────────────────────────────────────");
    Console.WriteLine(enhanced);
    Console.WriteLine("─────────────────────────────────────────");
}

static async Task TestStyleVariations(AIService aiService)
{
    Console.WriteLine("\n=== Style Variations ===\n");

    var styles = new[] { "3D", "Minimal", "Gradient", "Glassmorphism" };
    var concept = "weather app";
    var colors = new List<string> { "#4A90E2", "#50C878" };

    foreach (var style in styles)
    {
        Console.WriteLine($"\n{style} Style:");
        Console.WriteLine("─────────────────────────────────────────");

        var request = new IconGenerator.Functions.Models.IconGenerationRequest
        {
            Keywords = concept,
            Style = style,
            Colors = colors
        };
        var enhanced = await aiService.EnhancePromptAsync(request);
        Console.WriteLine(enhanced);

        await Task.Delay(1000); // Rate limiting
    }
}

static async Task TestColorPalette(AIService aiService)
{
    Console.WriteLine("\n=== Color Palette Test ===\n");

    var request = new IconGenerator.Functions.Models.IconGenerationRequest
    {
        Keywords = "social media app",
        Style = "Gradient",
        Colors = new List<string> { "#FF6B6B", "#4ECDC4", "#45B7D1" }
    };

    Console.WriteLine($"Concept: {request.Keywords}");
    Console.WriteLine($"Style: {request.Style}");
    Console.WriteLine($"Palette: {string.Join(", ", request.Colors)}\n");

    var enhanced = await aiService.EnhancePromptAsync(request);

    Console.WriteLine("Enhanced with Color Guidance:");
    Console.WriteLine("─────────────────────────────────────────");
    Console.WriteLine(enhanced);
    Console.WriteLine("─────────────────────────────────────────");
}

static void TestQualityScores(PromptEngineeringService promptService)
{
    Console.WriteLine("\n=== Quality Score Evaluation ===\n");

    var prompts = new[]
    {
        ("Good", "Create a modern, minimalist app icon for a fitness tracker featuring a stylized heart rate monitor with clean lines, vibrant green and blue gradient colors, and a professional rounded square shape optimized for iOS and Android platforms."),
        ("Average", "App icon for fitness tracker with heart monitor and nice colors"),
        ("Poor", "fitness icon")
    };

    foreach (var (quality, prompt) in prompts)
    {
        var score = promptService.AnalyzePromptQuality(prompt);
        Console.WriteLine($"{quality} Prompt (Score: {score.OverallScore:F1}%):");
        Console.WriteLine($"  \"{prompt.Substring(0, Math.Min(60, prompt.Length))}...\"");
        Console.WriteLine();
    }
}

static void TestPromptVariations(PromptEngineeringService promptService)
{
    Console.WriteLine("\n=== Prompt A/B Testing Variations ===\n");

    var request = new IconGenerator.Functions.Models.IconGenerationRequest
    {
        Keywords = "fitness tracker app icon",
        Style = "3D",
        Colors = new List<string> { "#FF5733", "#33FF57" }
    };

    var variations = promptService.BuildVariationPrompts(request, 3);

    Console.WriteLine("Variation 1 (Literal):");
    Console.WriteLine("─────────────────────────────────────────");
    Console.WriteLine(variations[0]);
    Console.WriteLine();

    Console.WriteLine("Variation 2 (Abstract):");
    Console.WriteLine("─────────────────────────────────────────");
    Console.WriteLine(variations[1]);
    Console.WriteLine();

    Console.WriteLine("Variation 3 (Metaphorical):");
    Console.WriteLine("─────────────────────────────────────────");
    Console.WriteLine(variations[2]);
    Console.WriteLine();
}
