namespace IconGenerator.Functions.Services;

using IconGenerator.Functions.Models;

public interface IAIService
{
    Task<string> EnhancePromptAsync(IconGenerationRequest request, CancellationToken cancellationToken = default);
    Task<string> GenerateIconAsync(string enhancedPrompt, string quality = "standard", CancellationToken cancellationToken = default);
}
