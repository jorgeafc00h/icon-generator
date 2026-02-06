namespace IconGenerator.Functions.Services;

using Azure;
using Azure.AI.OpenAI;
using IconGenerator.Functions.Models;
using IconGenerator.Functions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class AIService : IAIService
{
    private readonly OpenAIClient _client;
    private readonly AzureOpenAIOptions _options;
    private readonly PromptEngineeringService _promptService;
    private readonly ILogger<AIService> _logger;

    public AIService(
        IOptions<AzureOpenAIOptions> options,
        PromptEngineeringService promptService,
        ILogger<AIService> logger)
    {
        _options = options.Value;
        _promptService = promptService;
        _logger = logger;

        var endpoint = new Uri(_options.Endpoint);
        var credential = new AzureKeyCredential(_options.ApiKey);
        _client = new OpenAIClient(endpoint, credential);
    }

    public async Task<string> EnhancePromptAsync(
        IconGenerationRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Use enhanced prompt engineering service with design knowledge
            var systemPrompt = _promptService.BuildIconSystemPrompt(request.Style);
            var userPrompt = _promptService.BuildIconUserPrompt(request);

            _logger.LogInformation(
                "Enhancing prompt for style: {Style}, keywords: {Keywords}",
                request.Style,
                request.Keywords);

            var chatCompletionsOptions = new ChatCompletionsOptions
            {
                DeploymentName = _options.Gpt4oMiniDeployment,
                Messages =
                {
                    new ChatRequestSystemMessage(systemPrompt),
                    new ChatRequestUserMessage(userPrompt)
                },
                Temperature = 0.7f,
                MaxTokens = 500 // Increased for more detailed prompts
            };

            var response = await _client.GetChatCompletionsAsync(chatCompletionsOptions, cancellationToken);
            var enhancedPrompt = response.Value.Choices[0].Message.Content;

            // Apply additional sanitization to the enhanced prompt
            enhancedPrompt = SanitizeEnhancedPrompt(enhancedPrompt ?? string.Empty);

            // Analyze prompt quality
            var qualityScore = _promptService.AnalyzePromptQuality(enhancedPrompt);
            _logger.LogInformation(
                "Enhanced prompt generated. Quality score: {Score}%\nPrompt: {Prompt}",
                qualityScore.OverallScore,
                enhancedPrompt);

            return enhancedPrompt;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enhancing prompt");
            // Return sanitized fallback
            return SanitizeEnhancedPrompt(request.Keywords);
        }
    }

    /// <summary>
    /// Sanitize enhanced prompts to avoid content filter triggers
    /// </summary>
    private string SanitizeEnhancedPrompt(string prompt)
    {
        if (string.IsNullOrEmpty(prompt)) return prompt;

        // Remove instruction-style language that triggers filters
        var patterns = new Dictionary<string, string>
        {
            // Remove all-caps emphasis
            { @"\b(MUST|REQUIRED|CRITICAL|IMPORTANT|NO|NEVER|ALWAYS)\b", "$1" },
            // Simplify constraint language
            { @"Your request (was rejected|must)", "Design should" },
            { @"not allowed", "avoided" },
            { @"safety system", "" },
            { @"content filter", "" },
            { @"(?i)strictly follow", "follow" },
            { @"(?i)ensure that", "" },
            // Reduce excessive detail that looks like jailbreak attempts
            { @"(adhering to|according to|following|strictly).{0,20}(guidelines|rules|constraints|policies)", "" }
        };

        var sanitized = prompt;
        foreach (var pattern in patterns)
        {
            sanitized = System.Text.RegularExpressions.Regex.Replace(
                sanitized, 
                pattern.Key, 
                pattern.Value, 
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        // Add edge-to-edge reinforcement if not present
        if (!sanitized.Contains("edge", StringComparison.OrdinalIgnoreCase) && 
            !sanitized.Contains("bleed", StringComparison.OrdinalIgnoreCase))
        {
            sanitized += " Icon extends to canvas edges with no padding or margins.";
        }

        // Limit prompt length to reduce filter triggers (max 400 chars for DALL-E prompts)
        if (sanitized.Length > 450)
        {
            // Try to preserve edge-to-edge language if present
            var edgeLanguage = System.Text.RegularExpressions.Regex.Match(
                sanitized, 
                @"(edge[^.]*\.|bleed[^.]*\.|no padding[^.]*\.)", 
                System.Text.RegularExpressions.RegexOptions.IgnoreCase).Value;
            
            sanitized = sanitized.Substring(0, 400 - edgeLanguage.Length) + " " + edgeLanguage;
        }

        // Clean up any double spaces or awkward formatting
        sanitized = System.Text.RegularExpressions.Regex.Replace(sanitized, @"\s+", " ").Trim();

        _logger.LogDebug("Sanitized prompt: {Prompt}", sanitized);
        return sanitized;
    }

    public async Task<string> GenerateIconAsync(
        string enhancedPrompt,
        string quality = "standard",
        CancellationToken cancellationToken = default)
    {
        try
        {
            var imageQuality = quality.ToLowerInvariant() == "hd"
                ? ImageGenerationQuality.Hd
                : ImageGenerationQuality.Standard;

            var imageGenerationOptions = new ImageGenerationOptions
            {
                DeploymentName = _options.DallE3Deployment,
                Prompt = enhancedPrompt,
                Size = ImageSize.Size1024x1024,
                Quality = imageQuality,
                Style = ImageGenerationStyle.Vivid
            };

            var response = await _client.GetImageGenerationsAsync(imageGenerationOptions, cancellationToken);
            var imageUrl = response.Value.Data[0].Url?.ToString();

            if (string.IsNullOrEmpty(imageUrl))
            {
                throw new InvalidOperationException("No image URL returned from DALL-E 3");
            }

            _logger.LogInformation("Generated icon URL: {Url}", imageUrl);
            return imageUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating icon");
            throw;
        }
    }
}
