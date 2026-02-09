namespace IconGenerator.Functions.Services;

using IconGenerator.Functions.Models;

public interface IAIService
{
    Task<string> EnhancePromptAsync(IconGenerationRequest request, CancellationToken cancellationToken = default);
    Task<string> EnhanceUIPromptAsync(AppResourcesGenerationRequest request, CancellationToken cancellationToken = default);
    Task<string> GenerateIconAsync(string enhancedPrompt, string quality = "standard", CancellationToken cancellationToken = default);
    Task<ChatResponse> ChatWithDesignerAsync(ChatSession chatSession, string userMessage, CancellationToken cancellationToken = default);

    // Story Image Generation
    Task<EnhanceImagePromptResponse> EnhanceImagePromptAsync(EnhanceImagePromptRequest request, CancellationToken cancellationToken = default);
    Task<GeneratedStoryImage> GenerateStoryImageAsync(string prompt, ImageStyle style, ImageQuality quality, int sceneNumber, CancellationToken cancellationToken = default);
}
