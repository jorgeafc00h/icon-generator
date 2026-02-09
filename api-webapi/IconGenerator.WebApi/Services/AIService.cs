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
    private readonly UIPromptEngineeringService _uiPromptService;
    private readonly ILogger<AIService> _logger;

    public AIService(
        IOptions<AzureOpenAIOptions> options,
        PromptEngineeringService promptService,
        UIPromptEngineeringService uiPromptService,
        ILogger<AIService> logger)
    {
        _options = options.Value;
        _promptService = promptService;
        _uiPromptService = uiPromptService;
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

    public async Task<string> EnhanceUIPromptAsync(
        AppResourcesGenerationRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var screenType = request.Options.ScreenTypes.FirstOrDefault();
            var platform = request.Options.TargetPlatform?.ToString() ?? request.Platforms.FirstOrDefault() ?? "iOS";
            var category = request.Options.AppCategory ?? "App";

            var systemPrompt = _uiPromptService.BuildUISystemPrompt(screenType, category, platform);
            var userPrompt = _uiPromptService.BuildUIUserPrompt(request);

            _logger.LogInformation(
                "Enhancing UI prompt for {Screen}, {Category}, {Platform}",
                screenType,
                category,
                platform);

            var chatCompletionsOptions = new ChatCompletionsOptions
            {
                DeploymentName = _options.Gpt4oMiniDeployment,
                Messages =
                {
                    new ChatRequestSystemMessage(systemPrompt),
                    new ChatRequestUserMessage(userPrompt)
                },
                Temperature = 0.7f,
                MaxTokens = 600 // Increased for detailed UI prompts
            };

            var response = await _client.GetChatCompletionsAsync(chatCompletionsOptions, cancellationToken);
            var enhancedPrompt = response.Value.Choices[0].Message.Content;
            enhancedPrompt = _uiPromptService.SanitizeUIPrompt(enhancedPrompt ?? string.Empty);

            var qualityScore = _uiPromptService.AnalyzeUIPromptQuality(enhancedPrompt);
            _logger.LogInformation(
                "Enhanced UI prompt. Quality: {Score}%\nPrompt: {Prompt}",
                qualityScore.OverallScore,
                enhancedPrompt);

            return enhancedPrompt;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enhancing UI prompt");
            throw;
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

    public async Task<ChatResponse> ChatWithDesignerAsync(
        ChatSession chatSession,
        string userMessage,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var systemPrompt = $@"You are an expert UI/UX designer assistant helping to create screens for {chatSession.AppName}, a {chatSession.AppCategory} app.

App Context:
- Name: {chatSession.AppName}
- Category: {chatSession.AppCategory}
- Platform: {chatSession.TargetPlatform}
- Brand Colors: {string.Join(", ", chatSession.BrandColors)}
- Screens already generated: {string.Join(", ", chatSession.GeneratedScreens.Select(s => s.ScreenType))}

Available screen types you can generate:
Login, Signup, Home, Dashboard, Profile, Settings, Onboarding, ProductList, ProductDetail, Cart, Checkout, Orders, PatientsList, PatientDetail, Appointments, CalendarSync, Feed, Detail, Search

Your responsibilities:
1. Understand user requests for new screens or modifications
2. Provide helpful design suggestions and guidance
3. When user requests a specific screen, respond with which screen type to generate
4. Be conversational and helpful

Response Format:
If user wants to generate a screen, you MUST include in your response:
GENERATE_SCREEN: [ScreenType]
CUSTOM_PROMPT: [any specific customizations they mentioned]

Example:
User: ""I need a checkout page with Apple Pay""
You: ""I'll create a checkout screen for you with Apple Pay integration! This will include payment options, order summary, and a secure checkout flow.
GENERATE_SCREEN: Checkout
CUSTOM_PROMPT: Include Apple Pay button prominently, show secure payment badges""

If they're just asking questions or want suggestions, respond conversationally without the GENERATE_SCREEN tag.";

            // Build chat messages for GPT
            var messages = new List<ChatRequestMessage>
            {
                new ChatRequestSystemMessage(systemPrompt)
            };

            // Add recent chat history (last 10 messages for context)
            foreach (var msg in chatSession.Messages.TakeLast(10))
            {
                if (msg.Role == "user")
                {
                    messages.Add(new ChatRequestUserMessage(msg.Content));
                }
                else if (msg.Role == "assistant")
                {
                    messages.Add(new ChatRequestAssistantMessage(msg.Content));
                }
            }

            // Add current user message
            messages.Add(new ChatRequestUserMessage(userMessage));

            var chatCompletionsOptions = new ChatCompletionsOptions
            {
                DeploymentName = _options.Gpt4oMiniDeployment,
                Temperature = 0.7f,
                MaxTokens = 800
            };

            // Add all messages to the options
            foreach (var message in messages)
            {
                chatCompletionsOptions.Messages.Add(message);
            }

            var response = await _client.GetChatCompletionsAsync(chatCompletionsOptions, cancellationToken);
            var aiResponse = response.Value.Choices[0].Message.Content ?? string.Empty;

            _logger.LogInformation("Chat AI Response: {Response}", aiResponse);

            // Parse response to check if screen generation is requested
            var chatResponse = new ChatResponse
            {
                Message = aiResponse
            };

            // Check for GENERATE_SCREEN tag
            if (aiResponse.Contains("GENERATE_SCREEN:", StringComparison.OrdinalIgnoreCase))
            {
                var lines = aiResponse.Split('\n');
                foreach (var line in lines)
                {
                    if (line.StartsWith("GENERATE_SCREEN:", StringComparison.OrdinalIgnoreCase))
                    {
                        var screenTypeStr = line.Substring("GENERATE_SCREEN:".Length).Trim();
                        if (Enum.TryParse<ScreenType>(screenTypeStr, true, out var screenType))
                        {
                            chatResponse.ShouldGenerateScreen = true;
                            chatResponse.ScreenType = screenType;
                        }
                    }
                    else if (line.StartsWith("CUSTOM_PROMPT:", StringComparison.OrdinalIgnoreCase))
                    {
                        chatResponse.CustomPromptAddition = line.Substring("CUSTOM_PROMPT:".Length).Trim();
                    }
                }

                // Clean the message to remove the tags
                chatResponse.Message = System.Text.RegularExpressions.Regex.Replace(
                    aiResponse,
                    @"GENERATE_SCREEN:.*?($|\n)|CUSTOM_PROMPT:.*?($|\n)",
                    "",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase
                ).Trim();
            }

            return chatResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in chat with designer");
            return new ChatResponse
            {
                Message = "I apologize, but I encountered an error. Could you please rephrase your request?",
                ShouldGenerateScreen = false
            };
        }
    }

    // ==================== Story Image Generation ====================

    public async Task<EnhanceImagePromptResponse> EnhanceImagePromptAsync(
        EnhanceImagePromptRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Enhancing image prompt for style: {Style}, scenes: {Scenes}",
                request.Style, request.NumberOfScenes);

            var styleDescription = GetStyleDescription(request.Style);

            var systemPrompt = $@"You are an expert prompt engineer for AI image generation (DALL-E 3).
Your task is to enhance user prompts to create consistent, high-quality {request.Style} style images.

Style Guidelines for {request.Style}:
{styleDescription}

Key Requirements:
1. Maintain character/subject consistency across all scenes
2. Include specific visual details (colors, lighting, composition)
3. Add artistic style keywords appropriate for {request.Style}
4. Ensure prompts are detailed but under 400 characters each
5. For multiple scenes, keep the character description identical across all scenes

{(string.IsNullOrWhiteSpace(request.CharacterDescription) ?
    "" :
    $"Character/Subject to maintain across scenes:\n{request.CharacterDescription}\n")}

Output Format:
1. First line: ENHANCED_PROMPT: [single enhanced version of user's prompt]
2. Following lines: SCENE_1: [prompt for scene 1], SCENE_2: [prompt for scene 2], etc.
3. Last line: CONSISTENCY: [key elements to maintain across all scenes]
4. Final line: KEYWORDS: [comma-separated style keywords]";

            var userPrompt = $@"User's request: {request.UserPrompt}
Number of scenes needed: {request.NumberOfScenes}
Style: {request.Style}

Please enhance this prompt and create {request.NumberOfScenes} scene variations maintaining visual consistency.";

            var chatCompletionsOptions = new ChatCompletionsOptions
            {
                DeploymentName = _options.Gpt4oMiniDeployment,
                Temperature = 0.7f,
                MaxTokens = 1000
            };
            chatCompletionsOptions.Messages.Add(new ChatRequestSystemMessage(systemPrompt));
            chatCompletionsOptions.Messages.Add(new ChatRequestUserMessage(userPrompt));

            var response = await _client.GetChatCompletionsAsync(chatCompletionsOptions, cancellationToken);
            var aiResponse = response.Value.Choices[0].Message.Content ?? string.Empty;

            _logger.LogInformation("GPT Image Prompt Enhancement: {Response}", aiResponse);

            // Parse the response
            var result = new EnhanceImagePromptResponse();
            var lines = aiResponse.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                if (line.StartsWith("ENHANCED_PROMPT:", StringComparison.OrdinalIgnoreCase))
                {
                    result.EnhancedPrompt = line.Substring("ENHANCED_PROMPT:".Length).Trim();
                }
                else if (line.StartsWith("SCENE_", StringComparison.OrdinalIgnoreCase))
                {
                    var scenePrompt = line.Substring(line.IndexOf(':') + 1).Trim();
                    result.ScenePrompts.Add(scenePrompt);
                }
                else if (line.StartsWith("CONSISTENCY:", StringComparison.OrdinalIgnoreCase))
                {
                    result.ConsistencyGuideline = line.Substring("CONSISTENCY:".Length).Trim();
                }
                else if (line.StartsWith("KEYWORDS:", StringComparison.OrdinalIgnoreCase))
                {
                    var keywords = line.Substring("KEYWORDS:".Length).Trim();
                    result.StyleKeywords = keywords.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(k => k.Trim())
                        .ToList();
                }
            }

            // Ensure we have the right number of scenes
            if (result.ScenePrompts.Count < request.NumberOfScenes && !string.IsNullOrWhiteSpace(result.EnhancedPrompt))
            {
                // Fill with variations of the enhanced prompt
                for (int i = result.ScenePrompts.Count; i < request.NumberOfScenes; i++)
                {
                    result.ScenePrompts.Add($"{result.EnhancedPrompt} - scene {i + 1}");
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enhancing image prompt");
            // Return basic response
            return new EnhanceImagePromptResponse
            {
                EnhancedPrompt = request.UserPrompt,
                ScenePrompts = Enumerable.Range(1, request.NumberOfScenes)
                    .Select(i => $"{request.UserPrompt} - scene {i}")
                    .ToList(),
                ConsistencyGuideline = request.CharacterDescription ?? "Maintain visual consistency",
                StyleKeywords = new List<string> { request.Style.ToString().ToLower() }
            };
        }
    }

    public async Task<GeneratedStoryImage> GenerateStoryImageAsync(
        string prompt,
        ImageStyle style,
        ImageQuality quality,
        int sceneNumber,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Generating story image - Scene {Scene}, Style: {Style}, Quality: {Quality}",
                sceneNumber, style, quality);

            // Add style-specific keywords to prompt
            var styleKeywords = GetStyleKeywords(style);
            var enhancedPrompt = $"{prompt}. {styleKeywords}";

            // Determine DALL-E 3 size based on quality
            var imageSize = quality switch
            {
                ImageQuality.HD => ImageSize.Size1792x1024,           // Landscape HD
                ImageQuality.Portrait => ImageSize.Size1024x1792,     // Portrait
                _ => ImageSize.Size1024x1024                          // Standard/Preview
            };

            var dalleQuality = quality == ImageQuality.Preview
                ? ImageGenerationQuality.Standard
                : ImageGenerationQuality.Hd;

            _logger.LogInformation("DALL-E 3 Request - Size: {Size}, Quality: {Quality}, Prompt: {Prompt}",
                imageSize, dalleQuality, enhancedPrompt);

            var imageGenerationOptions = new ImageGenerationOptions
            {
                DeploymentName = _options.DallE3Deployment,
                Prompt = enhancedPrompt,
                Size = imageSize,
                Quality = dalleQuality,
                Style = ImageGenerationStyle.Vivid
            };

            var imageResponse = await _client.GetImageGenerationsAsync(imageGenerationOptions, cancellationToken);
            var imageUrl = imageResponse.Value.Data[0].Url?.ToString() ?? string.Empty;

            _logger.LogInformation("Image generated successfully: {Url}", imageUrl);

            return new GeneratedStoryImage
            {
                Id = Guid.NewGuid().ToString(),
                ImageUrl = imageUrl,
                Prompt = enhancedPrompt,
                Quality = quality,
                Style = style,
                SceneNumber = sceneNumber,
                CreatedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating story image");
            throw;
        }
    }

    private string GetStyleDescription(ImageStyle style)
    {
        return style switch
        {
            ImageStyle.Cartoon => "Colorful, exaggerated features, bold outlines, playful and vibrant. Think Disney or Pixar style.",
            ImageStyle.Illustration => "Traditional illustration with artistic details, rich colors, and storytelling elements. Children's book quality.",
            ImageStyle.Watercolor => "Soft, flowing colors with gentle gradients. Dreamy, ethereal quality with visible brush strokes.",
            ImageStyle.DigitalArt => "Modern digital painting with clean lines, vibrant colors, and polished finish.",
            ImageStyle.PixelArt => "Retro 8-bit or 16-bit style with visible pixels, limited color palette, nostalgic gaming aesthetic.",
            ImageStyle.Realistic => "Photorealistic details, accurate lighting and shadows, lifelike textures.",
            ImageStyle.Sketch => "Hand-drawn appearance with visible pencil or pen strokes, artistic and loose.",
            ImageStyle.ThreeD => "3D rendered with depth, lighting, and realistic materials. Pixar-like quality.",
            ImageStyle.Anime => "Japanese anime/manga style with large expressive eyes, dynamic poses, vibrant colors.",
            ImageStyle.Comic => "Comic book style with bold lines, dynamic action, dramatic shadows and highlights.",
            ImageStyle.Minimalist => "Simple, clean design with limited colors and shapes. Focus on essential elements only.",
            ImageStyle.VintagePoster => "Retro poster art with limited color palette, bold typography, nostalgic 1950s-1970s aesthetic.",
            _ => "High-quality artistic rendering"
        };
    }

    private string GetStyleKeywords(ImageStyle style)
    {
        return style switch
        {
            ImageStyle.Cartoon => "cartoon style, vibrant colors, bold outlines, playful, Disney-style animation",
            ImageStyle.Illustration => "children's book illustration, detailed artwork, storybook quality, professional illustration",
            ImageStyle.Watercolor => "watercolor painting, soft colors, artistic brush strokes, dreamy atmosphere",
            ImageStyle.DigitalArt => "digital art, modern illustration, clean lines, polished finish",
            ImageStyle.PixelArt => "pixel art, retro gaming style, 16-bit graphics, nostalgic aesthetic",
            ImageStyle.Realistic => "photorealistic, highly detailed, accurate lighting, lifelike quality",
            ImageStyle.Sketch => "pencil sketch, hand-drawn, artistic linework, sketchy style",
            ImageStyle.ThreeD => "3D rendered, Pixar-style, realistic lighting and materials, CGI quality",
            ImageStyle.Anime => "anime style, manga art, expressive characters, vibrant anime aesthetic",
            ImageStyle.Comic => "comic book art, dynamic composition, bold inking, graphic novel style",
            ImageStyle.Minimalist => "minimalist design, simple shapes, clean composition, essential elements only",
            ImageStyle.VintagePoster => "vintage poster art, retro design, classic advertising style, nostalgic",
            _ => "high-quality artistic style"
        };
    }
}
