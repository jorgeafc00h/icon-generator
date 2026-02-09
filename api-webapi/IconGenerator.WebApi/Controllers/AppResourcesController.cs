namespace IconGenerator.WebApi.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using IconGenerator.Functions.Models;
using IconGenerator.Functions.Services;
using IconGenerator.Functions.Options;

[ApiController]
[Route("api/app-resources")]
[Authorize]
public class AppResourcesController : ControllerBase
{
    private readonly IAIService _aiService;
    private readonly IStorageService _storageService;
    private readonly IDatabaseService _databaseService;
    private readonly ILogger<AppResourcesController> _logger;
    private readonly AppSettingsOptions _appSettings;

    public AppResourcesController(
        IAIService aiService,
        IStorageService storageService,
        IDatabaseService databaseService,
        ILogger<AppResourcesController> logger,
        IOptions<AppSettingsOptions> appSettings)
    {
        _aiService = aiService;
        _storageService = storageService;
        _databaseService = databaseService;
        _logger = logger;
        _appSettings = appSettings.Value;
    }

    /// <summary>
    /// Generate complete app resources (icons + screen mockups)
    /// </summary>
    [HttpPost("generate")]
    public async Task<IActionResult> GenerateAppResources([FromBody] AppResourcesGenerationRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generate app resources endpoint triggered");

        try
        {
            // Get user ID from JWT claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "Invalid authentication token" });
            }

            // Get user from database
            var user = await _databaseService.GetUserByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                return NotFound(new { error = "User not found" });
            }

            // Calculate required credits (1 credit per screen)
            var requiredCredits = request.Options.ScreenTypes.Count;

            // Check if user has enough credits
            if (!(user.Metadata?.IsUnlimited ?? false) && user.Credits < requiredCredits)
            {
                return BadRequest(new
                {
                    error = $"Insufficient credits. You need {requiredCredits} credits but only have {user.Credits}.",
                    requiredCredits,
                    availableCredits = user.Credits
                });
            }

            // Generate session ID for this generation batch
            var sessionId = Guid.NewGuid().ToString();

            // Generate screens
            var generatedScreens = new List<GeneratedScreen>();
            var totalCost = 0m;

            foreach (var screenType in request.Options.ScreenTypes)
            {
                try
                {
                    // Create request for this specific screen
                    var screenRequest = new AppResourcesGenerationRequest
                    {
                        Platforms = request.Platforms,
                        Options = new AppResourcesOptions
                        {
                            ScreenTypes = new List<ScreenType> { screenType },
                            AppName = request.Options.AppName,
                            AppCategory = request.Options.AppCategory,
                            BrandPrimaryColor = request.Options.BrandPrimaryColor,
                            BrandSecondaryColor = request.Options.BrandSecondaryColor,
                            TargetPlatform = request.Options.TargetPlatform,
                            EnableAccessibilityMode = request.Options.EnableAccessibilityMode
                        }
                    };

                    // Enhance prompt using UIPromptEngineeringService
                    var enhancedPrompt = await _aiService.EnhanceUIPromptAsync(screenRequest, cancellationToken);
                    _logger.LogInformation("Enhanced prompt for screen {ScreenType}: {Prompt}", screenType, enhancedPrompt);

                    // Generate image using DALL-E
                    var imageUrl = await _aiService.GenerateIconAsync(enhancedPrompt, "standard", cancellationToken);

                    // Upload to storage
                    var screenId = $"{sessionId}-{screenType.ToString().ToLower()}";
                    var storedUrl = await _storageService.UploadImageAsync(imageUrl, userId, screenId);

                    generatedScreens.Add(new GeneratedScreen
                    {
                        Id = screenId,
                        ScreenType = screenType,
                        ImageUrl = storedUrl,
                        Prompt = enhancedPrompt,
                        CreatedAt = DateTime.UtcNow
                    });

                    totalCost += 0.04m; // $0.04 per screen (standard quality)
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error generating screen {ScreenType}", screenType);
                    // Continue with other screens even if one fails
                }
            }

            if (generatedScreens.Count == 0)
            {
                return StatusCode(500, new { error = "Failed to generate any screens" });
            }

            // Deduct credits if not unlimited
            if (!(user.Metadata?.IsUnlimited ?? false))
            {
                user.Credits -= generatedScreens.Count;
                user.Metadata ??= new UserMetadata();
                user.Metadata.TotalCreditsSpent += generatedScreens.Count;
                user.Metadata.TotalIconsGenerated += generatedScreens.Count;

                await _databaseService.UpdateUserAsync(user, cancellationToken);
            }

            // Create chat session for iterative generation
            var chatSession = new ChatSession
            {
                Id = sessionId,
                UserId = userId,
                AppName = request.Options.AppName,
                AppCategory = request.Options.AppCategory,
                BrandColors = new List<string>
                {
                    request.Options.BrandPrimaryColor ?? "#4A90E2",
                    request.Options.BrandSecondaryColor ?? "#50C878"
                },
                TargetPlatform = request.Options.TargetPlatform?.ToString() ?? "iOS",
                GeneratedScreens = generatedScreens,
                Messages = new List<ChatMessage>
                {
                    new ChatMessage
                    {
                        Role = "system",
                        Content = $"You are an expert UI/UX designer helping to create screens for {request.Options.AppName}, a {request.Options.AppCategory} app. You can generate new screens, modify existing ones, or provide design suggestions.",
                        Timestamp = DateTime.UtcNow
                    }
                },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Save chat session
            await _databaseService.SaveChatSessionAsync(chatSession, cancellationToken);

            _logger.LogInformation("Generated {Count} screens for user {UserId}. Total cost: ${Cost}",
                generatedScreens.Count, userId, totalCost);

            return Ok(new
            {
                sessionId,
                screens = generatedScreens,
                creditsUsed = generatedScreens.Count,
                remainingCredits = (user.Metadata?.IsUnlimited ?? false) ? -1 : user.Credits,
                totalCost
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating app resources");
            return StatusCode(500, new { error = "Failed to generate app resources", details = ex.Message });
        }
    }

    /// <summary>
    /// Chat with AI to generate new screens or modify existing ones
    /// </summary>
    [HttpPost("chat")]
    public async Task<IActionResult> ChatGenerateScreen([FromBody] ChatRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Chat generate screen endpoint triggered");

        try
        {
            // Get user ID from JWT claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "Invalid authentication token" });
            }

            // Get user from database
            var user = await _databaseService.GetUserByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                return NotFound(new { error = "User not found" });
            }

            // Get chat session
            var chatSession = await _databaseService.GetChatSessionAsync(request.SessionId, cancellationToken);
            if (chatSession == null || chatSession.UserId != userId)
            {
                return NotFound(new { error = "Chat session not found" });
            }

            // Add user message to chat history
            chatSession.Messages.Add(new ChatMessage
            {
                Role = "user",
                Content = request.Message,
                Timestamp = DateTime.UtcNow
            });

            // Call Azure OpenAI to understand the request and generate response
            var chatResponse = await _aiService.ChatWithDesignerAsync(chatSession, request.Message, cancellationToken);

            // Add AI response to chat history
            chatSession.Messages.Add(new ChatMessage
            {
                Role = "assistant",
                Content = chatResponse.Message,
                Timestamp = DateTime.UtcNow
            });

            // Check if AI wants to generate a new screen
            if (chatResponse.ShouldGenerateScreen && chatResponse.ScreenType.HasValue)
            {
                // Check credits
                if (!(user.Metadata?.IsUnlimited ?? false) && user.Credits < 1)
                {
                    return BadRequest(new
                    {
                        error = "Insufficient credits",
                        message = chatResponse.Message,
                        requiresCredits = true
                    });
                }

                try
                {
                    // Generate the requested screen
                    var screenRequest = new AppResourcesGenerationRequest
                    {
                        Platforms = new List<string> { chatSession.TargetPlatform },
                        Options = new AppResourcesOptions
                        {
                            ScreenTypes = new List<ScreenType> { chatResponse.ScreenType.Value },
                            AppName = chatSession.AppName,
                            AppCategory = chatSession.AppCategory,
                            BrandPrimaryColor = chatSession.BrandColors.FirstOrDefault(),
                            BrandSecondaryColor = chatSession.BrandColors.Skip(1).FirstOrDefault(),
                            TargetPlatform = Enum.Parse<Platform>(chatSession.TargetPlatform),
                            EnableAccessibilityMode = true
                        }
                    };

                    // Enhance prompt with user's specific request
                    var enhancedPrompt = await _aiService.EnhanceUIPromptAsync(screenRequest, cancellationToken);

                    // Append user's custom requirements if provided
                    if (!string.IsNullOrEmpty(chatResponse.CustomPromptAddition))
                    {
                        enhancedPrompt += " " + chatResponse.CustomPromptAddition;
                    }

                    // Generate image
                    var imageUrl = await _aiService.GenerateIconAsync(enhancedPrompt, "standard", cancellationToken);

                    // Upload to storage
                    var screenId = $"{chatSession.Id}-chat-{DateTime.UtcNow.Ticks}";
                    var storedUrl = await _storageService.UploadImageAsync(imageUrl, userId, screenId);

                    var generatedScreen = new GeneratedScreen
                    {
                        Id = screenId,
                        ScreenType = chatResponse.ScreenType.Value,
                        ImageUrl = storedUrl,
                        Prompt = enhancedPrompt,
                        CreatedAt = DateTime.UtcNow
                    };

                    chatSession.GeneratedScreens.Add(generatedScreen);

                    // Deduct credit
                    if (!(user.Metadata?.IsUnlimited ?? false))
                    {
                        user.Credits -= 1;
                        user.Metadata ??= new UserMetadata();
                        user.Metadata.TotalCreditsSpent += 1;
                        user.Metadata.TotalIconsGenerated += 1;

                        await _databaseService.UpdateUserAsync(user, cancellationToken);
                    }

                    chatResponse.GeneratedScreen = generatedScreen;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error generating screen in chat");
                    chatResponse.Message += "\n\nSorry, I encountered an error while generating that screen. Please try again.";
                }
            }

            // Update chat session
            chatSession.UpdatedAt = DateTime.UtcNow;
            await _databaseService.UpdateChatSessionAsync(chatSession, cancellationToken);

            return Ok(new
            {
                message = chatResponse.Message,
                generatedScreen = chatResponse.GeneratedScreen,
                shouldGenerateScreen = chatResponse.ShouldGenerateScreen,
                remainingCredits = (user.Metadata?.IsUnlimited ?? false) ? -1 : user.Credits,
                chatSession = new
                {
                    chatSession.Id,
                    totalScreens = chatSession.GeneratedScreens.Count,
                    screens = chatSession.GeneratedScreens
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in chat generate screen");
            return StatusCode(500, new { error = "Failed to process chat request", details = ex.Message });
        }
    }

    /// <summary>
    /// Get chat session details
    /// </summary>
    [HttpGet("sessions/{sessionId}")]
    public async Task<IActionResult> GetChatSession(string sessionId, CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "Invalid authentication token" });
            }

            var chatSession = await _databaseService.GetChatSessionAsync(sessionId, cancellationToken);
            if (chatSession == null || chatSession.UserId != userId)
            {
                return NotFound(new { error = "Chat session not found" });
            }

            return Ok(chatSession);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting chat session");
            return StatusCode(500, new { error = "Failed to get chat session" });
        }
    }

    /// <summary>
    /// Get user's chat sessions
    /// </summary>
    [HttpGet("sessions")]
    public async Task<IActionResult> GetUserChatSessions(CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "Invalid authentication token" });
            }

            var sessions = await _databaseService.GetUserChatSessionsAsync(userId, 50, cancellationToken);
            return Ok(sessions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user chat sessions");
            return StatusCode(500, new { error = "Failed to get chat sessions" });
        }
    }
}
