using Xunit;
using IconGenerator.Functions.Services;
using IconGenerator.Functions.Models;

namespace IconGenerator.Tests.Unit;

/// <summary>
/// Unit tests for UIPromptEngineeringService
/// </summary>
public class UIPromptEngineeringServiceTests
{
    private readonly UIPromptEngineeringService _service;

    public UIPromptEngineeringServiceTests()
    {
        _service = new UIPromptEngineeringService();
    }

    [Fact(DisplayName = "Should build UI system prompt with Material Design for Android")]
    public void ShouldBuildUISystemPromptWithMaterialDesign()
    {
        // Arrange
        var screenType = ScreenType.Login;
        var category = "healthcare";
        var platform = "Android";

        // Act
        var systemPrompt = _service.BuildUISystemPrompt(screenType, category, platform);

        // Assert
        Assert.NotNull(systemPrompt);
        Assert.Contains("Material Design", systemPrompt, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("8px grid", systemPrompt, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("accessibility", systemPrompt, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("healthcare", systemPrompt, StringComparison.OrdinalIgnoreCase);
    }

    [Fact(DisplayName = "Should build UI system prompt with iOS HIG for iOS")]
    public void ShouldBuildUISystemPromptWithiOSHIG()
    {
        // Arrange
        var screenType = ScreenType.Dashboard;
        var category = "healthcare";
        var platform = "iOS";

        // Act
        var systemPrompt = _service.BuildUISystemPrompt(screenType, category, platform);

        // Assert
        Assert.NotNull(systemPrompt);
        Assert.Contains("iOS", systemPrompt, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("44x44", systemPrompt); // iOS touch target
        Assert.Contains("healthcare", systemPrompt, StringComparison.OrdinalIgnoreCase);
    }

    [Fact(DisplayName = "Should build UI user prompt with brand colors")]
    public void ShouldBuildUIUserPromptWithBrandColors()
    {
        // Arrange
        var request = new AppResourcesGenerationRequest
        {
            Platforms = new List<string> { "iOS" },
            Options = new AppResourcesOptions
            {
                ScreenTypes = new List<ScreenType> { ScreenType.Login },
                AppName = "HealthCare Pro",
                AppCategory = "healthcare",
                BrandPrimaryColor = "#4A90E2",
                BrandSecondaryColor = "#50C878",
                TargetPlatform = Platform.iOS
            }
        };

        // Act
        var userPrompt = _service.BuildUIUserPrompt(request);

        // Assert
        Assert.NotNull(userPrompt);
        Assert.Contains("HealthCare Pro", userPrompt);
        Assert.Contains("#4A90E2", userPrompt);
        Assert.Contains("#50C878", userPrompt);
        Assert.Contains("healthcare", userPrompt, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Login", userPrompt);
    }

    [Fact(DisplayName = "Should get UI components for Login screen")]
    public void ShouldGetUIComponentsForLoginScreen()
    {
        // Arrange
        var screenType = ScreenType.Login;
        var category = "healthcare";

        // Act
        var components = _service.GetUIComponentsForScreen(screenType, category);

        // Assert
        Assert.NotNull(components);
        Assert.Contains("email", components, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("password", components, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Sign In", components);
        Assert.Contains("Google", components);
        Assert.Contains("Apple", components);
        Assert.Contains("HIPAA", components); // Healthcare-specific
    }

    [Fact(DisplayName = "Should get UI components for PatientsList screen")]
    public void ShouldGetUIComponentsForPatientsListScreen()
    {
        // Arrange
        var screenType = ScreenType.PatientsList;
        var category = "healthcare";

        // Act
        var components = _service.GetUIComponentsForScreen(screenType, category);

        // Assert
        Assert.NotNull(components);
        Assert.Contains("Search", components);
        Assert.Contains("patient", components, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("48-64px", components); // Patient photo size
        Assert.Contains("status", components, StringComparison.OrdinalIgnoreCase);
    }

    [Fact(DisplayName = "Should get navigation pattern for healthcare category")]
    public void ShouldGetNavigationPatternForHealthcare()
    {
        // Arrange
        var category = "healthcare";
        var screenType = ScreenType.Dashboard;

        // Act
        var navigation = _service.GetNavigationPattern(category, screenType);

        // Assert
        Assert.NotNull(navigation);
        Assert.Contains("Bottom", navigation);
        Assert.Contains("Dashboard", navigation);
        Assert.Contains("Patients", navigation);
        Assert.Contains("Appointments", navigation);
    }

    [Fact(DisplayName = "Should get layout pattern for ProductList")]
    public void ShouldGetLayoutPatternForProductList()
    {
        // Arrange
        var screenType = ScreenType.ProductList;

        // Act
        var layout = _service.GetLayoutPattern(screenType);

        // Assert
        Assert.NotNull(layout);
        Assert.Contains("GRID", layout, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Columns: 2", layout, StringComparison.OrdinalIgnoreCase);
    }

    [Fact(DisplayName = "Should get 8px spacing guidance")]
    public void ShouldGet8pxSpacingGuidance()
    {
        // Arrange
        var platform = "iOS";

        // Act
        var spacing = _service.GetSpacingGuidance(platform);

        // Assert
        Assert.NotNull(spacing);
        Assert.Contains("8", spacing);
        Assert.Contains("grid", spacing, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("16", spacing);
        Assert.Contains("24", spacing);
        Assert.Contains("points", spacing); // iOS uses points
    }

    [Fact(DisplayName = "Should analyze UI prompt quality")]
    public void ShouldAnalyzeUIPromptQuality()
    {
        // Arrange
        var prompt = @"Create an iOS Login screen with:
            - Email field 56px height
            - Password field 56px height
            - Sign In button 48px height
            - 8px grid system spacing
            - Bottom tab navigation
            - Material Design components
            - 44x44px touch targets for accessibility
            - WCAG AA contrast ratios
            - Layout using single column";

        // Act
        var score = _service.AnalyzeUIPromptQuality(prompt);

        // Assert
        Assert.True(score.HasComponentSpecification);
        Assert.True(score.HasLayoutGuidance);
        Assert.True(score.HasSpacingSystem);
        Assert.True(score.HasAccessibilityConsiderations);
        Assert.True(score.HasPlatformGuidelines);
        Assert.True(score.HasNavigationPattern);
        Assert.True(score.OverallScore >= 95); // Should be near perfect
    }

    [Fact(DisplayName = "Should sanitize UI prompt")]
    public void ShouldSanitizeUIPrompt()
    {
        // Arrange
        var prompt = "You MUST create a design!! The design should NEVER use red!! This is CRITICAL!!";

        // Act
        var sanitized = _service.SanitizeUIPrompt(prompt);

        // Assert
        Assert.NotNull(sanitized);
        Assert.DoesNotContain("MUST", sanitized);
        Assert.DoesNotContain("NEVER", sanitized);
        Assert.DoesNotContain("CRITICAL", sanitized);
        Assert.DoesNotContain("!!", sanitized);
    }

    [Fact(DisplayName = "Should build multi-screen flow with continuity")]
    public void ShouldBuildMultiScreenFlowWithContinuity()
    {
        // Arrange
        var screens = new List<ScreenType>
        {
            ScreenType.Login,
            ScreenType.Dashboard,
            ScreenType.Profile
        };
        var category = "healthcare";
        var appName = "HealthCare Pro";
        var platform = "iOS";

        // Act
        var flow = _service.BuildMultiScreenFlow(screens, category, appName, platform);

        // Assert
        Assert.Equal(3, flow.Count);
        Assert.Contains("screen 1 of 3", flow[0].Item2, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("screen 2 of 3", flow[1].Item2, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("screen 3 of 3", flow[2].Item2, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("visual continuity", flow[1].Item2, StringComparison.OrdinalIgnoreCase);
    }

    [Fact(DisplayName = "Should build UI variation prompts")]
    public void ShouldBuildUIVariationPrompts()
    {
        // Arrange
        var request = new AppResourcesGenerationRequest
        {
            Platforms = new List<string> { "iOS" },
            Options = new AppResourcesOptions
            {
                ScreenTypes = new List<ScreenType> { ScreenType.Dashboard },
                AppName = "MyApp",
                AppCategory = "productivity",
                TargetPlatform = Platform.iOS
            }
        };

        // Act
        var variations = _service.BuildUIVariationPrompts(request, 3);

        // Assert
        Assert.Equal(3, variations.Count);
        Assert.Contains("MINIMALIST", variations[0], StringComparison.OrdinalIgnoreCase);
        Assert.Contains("VISUAL-FIRST", variations[1], StringComparison.OrdinalIgnoreCase);
        Assert.Contains("DATA-DENSE", variations[2], StringComparison.OrdinalIgnoreCase);
    }

    [Fact(DisplayName = "Should get state variation for empty state")]
    public void ShouldGetStateVariationForEmptyState()
    {
        // Arrange
        var screenType = ScreenType.PatientsList;
        var state = "empty";

        // Act
        var statePrompt = _service.GetStateVariation(screenType, state);

        // Assert
        Assert.NotNull(statePrompt);
        Assert.Contains("empty", statePrompt, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("illustration", statePrompt, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("200x200", statePrompt);
    }

    [Theory(DisplayName = "Should handle all screen types")]
    [InlineData(ScreenType.Login)]
    [InlineData(ScreenType.Signup)]
    [InlineData(ScreenType.Home)]
    [InlineData(ScreenType.Dashboard)]
    [InlineData(ScreenType.Profile)]
    [InlineData(ScreenType.Settings)]
    [InlineData(ScreenType.Onboarding)]
    [InlineData(ScreenType.ProductList)]
    [InlineData(ScreenType.ProductDetail)]
    [InlineData(ScreenType.Cart)]
    [InlineData(ScreenType.Checkout)]
    [InlineData(ScreenType.Orders)]
    [InlineData(ScreenType.PatientsList)]
    [InlineData(ScreenType.PatientDetail)]
    [InlineData(ScreenType.Appointments)]
    [InlineData(ScreenType.CalendarSync)]
    [InlineData(ScreenType.Feed)]
    [InlineData(ScreenType.Detail)]
    [InlineData(ScreenType.Search)]
    public void ShouldHandleAllScreenTypes(ScreenType screenType)
    {
        // Arrange
        var category = "healthcare";

        // Act
        var components = _service.GetUIComponentsForScreen(screenType, category);

        // Assert
        Assert.NotNull(components);
        Assert.NotEmpty(components);
    }

    [Theory(DisplayName = "Should handle all platforms")]
    [InlineData("iOS")]
    [InlineData("Android")]
    [InlineData("Web")]
    [InlineData("MacOS")]
    public void ShouldHandleAllPlatforms(string platform)
    {
        // Arrange
        var screenType = ScreenType.Dashboard;
        var category = "healthcare";

        // Act
        var systemPrompt = _service.BuildUISystemPrompt(screenType, category, platform);

        // Assert
        Assert.NotNull(systemPrompt);
        Assert.NotEmpty(systemPrompt);
        Assert.Contains("8px", systemPrompt, StringComparison.OrdinalIgnoreCase);
    }
}
