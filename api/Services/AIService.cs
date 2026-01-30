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

            // Analyze prompt quality
            var qualityScore = _promptService.AnalyzePromptQuality(enhancedPrompt ?? string.Empty);
            _logger.LogInformation(
                "Enhanced prompt generated. Quality score: {Score}%\nPrompt: {Prompt}",
                qualityScore.OverallScore,
                enhancedPrompt);

            return enhancedPrompt ?? request.Keywords;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enhancing prompt");
            return request.Keywords; // Fallback to original if enhancement fails
        }
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
