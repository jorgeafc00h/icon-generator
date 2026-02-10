namespace IconGenerator.WebApi.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using IconGenerator.Functions.Models;
using IconGenerator.Functions.Services;

[ApiController]
[Route("api/images")]
[Authorize]
public class ImagesController : ControllerBase
{
    private readonly IAIService _aiService;
    private readonly IStorageService _storageService;
    private readonly IDatabaseService _databaseService;
    private readonly ILogger<ImagesController> _logger;

    public ImagesController(
        IAIService aiService,
        IStorageService storageService,
        IDatabaseService databaseService,
        ILogger<ImagesController> logger)
    {
        _aiService = aiService;
        _storageService = storageService;
        _databaseService = databaseService;
        _logger = logger;
    }

    /// <summary>
    /// Enhance user's image prompt using GPT for consistency and quality
    /// </summary>
    [HttpPost("enhance-prompt")]
    [AllowAnonymous] // Allow prompt enhancement without authentication
    public async Task<IActionResult> EnhanceImagePrompt(
        [FromBody] EnhanceImagePromptRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Enhance image prompt request - Prompt: {Prompt}, Style: {Style}, Scenes: {Scenes}",
                request.UserPrompt, request.Style, request.NumberOfScenes);

            // Validate request
            if (string.IsNullOrWhiteSpace(request.UserPrompt))
            {
                return BadRequest(new { error = "UserPrompt is required" });
            }

            if (request.NumberOfScenes < 1 || request.NumberOfScenes > 10)
            {
                return BadRequest(new { error = "NumberOfScenes must be between 1 and 10" });
            }

            var enhancedResponse = await _aiService.EnhanceImagePromptAsync(request, cancellationToken);

            return Ok(enhancedResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enhancing image prompt");
            return StatusCode(500, new { error = "Failed to enhance prompt", details = ex.Message });
        }
    }

    /// <summary>
    /// Generate preview images (lower quality for review)
    /// </summary>
    [HttpPost("generate-preview")]
    public async Task<IActionResult> GeneratePreviewImages(
        [FromBody] StoryImageGenerationRequest request,
        CancellationToken cancellationToken)
    {
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

            // Calculate required credits (0.5 credit per preview image)
            var requiredCredits = (int)Math.Ceiling(request.NumberOfImages * 0.5m);

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

            _logger.LogInformation("Generating {Count} preview images for user {UserId}",
                request.NumberOfImages, userId);

            // Generate session ID
            var sessionId = Guid.NewGuid().ToString();
            var generatedImages = new List<GeneratedStoryImage>();

            // Enhance prompts if requested
            List<string> scenePrompts;
            if (request.UseEnhancedPrompts && request.SceneDescriptions.Count == 0)
            {
                var enhanceRequest = new EnhanceImagePromptRequest
                {
                    UserPrompt = request.BasePrompt,
                    Style = request.Style,
                    CharacterDescription = request.CharacterDescription,
                    NumberOfScenes = request.NumberOfImages
                };

                var enhancedResponse = await _aiService.EnhanceImagePromptAsync(enhanceRequest, cancellationToken);
                scenePrompts = enhancedResponse.ScenePrompts;
            }
            else
            {
                scenePrompts = request.SceneDescriptions.Count > 0
                    ? request.SceneDescriptions
                    : Enumerable.Range(1, request.NumberOfImages)
                        .Select(i => $"{request.BasePrompt} - scene {i}")
                        .ToList();
            }

            // Generate images
            for (int i = 0; i < request.NumberOfImages; i++)
            {
                var scenePrompt = scenePrompts[Math.Min(i, scenePrompts.Count - 1)];

                // Add character consistency if provided
                if (!string.IsNullOrWhiteSpace(request.CharacterDescription))
                {
                    scenePrompt = $"{request.CharacterDescription}. {scenePrompt}";
                }

                // Generate with DALL-E (temporary URL)
                var image = await _aiService.GenerateStoryImageAsync(
                    scenePrompt,
                    request.Style,
                    ImageQuality.Preview,
                    i + 1,
                    cancellationToken
                );

                _logger.LogInformation("Generated preview image {Index}/{Total}, uploading to storage...",
                    i + 1, request.NumberOfImages);

                // Upload to Azure Storage for permanent storage
                var storedUrl = await _storageService.UploadImageAsync(
                    image.ImageUrl,
                    userId,
                    image.Id,
                    cancellationToken
                );

                // Update image URL to permanent storage URL
                image.ImageUrl = storedUrl;
                image.SceneDescription = scenePrompt;
                generatedImages.Add(image);

                _logger.LogInformation("Uploaded image {Index}/{Total} to storage: {Url}",
                    i + 1, request.NumberOfImages, storedUrl);
            }

            // Deduct credits
            if (!(user.Metadata?.IsUnlimited ?? false))
            {
                user.Credits -= requiredCredits;
                user.Metadata ??= new UserMetadata();
                user.Metadata.TotalCreditsSpent += requiredCredits;
                await _databaseService.UpdateUserAsync(user, cancellationToken);
            }

            // Create session for tracking
            var session = new StoryImageSession
            {
                Id = sessionId,
                UserId = userId,
                BasePrompt = request.BasePrompt,
                Style = request.Style,
                CharacterDescription = request.CharacterDescription,
                Images = generatedImages
            };

            // TODO: Save session to database when we add StoryImageSession container

            var response = new StoryImageGenerationResponse
            {
                SessionId = sessionId,
                Images = generatedImages,
                CreditsUsed = requiredCredits,
                RemainingCredits = (user.Metadata?.IsUnlimited ?? false) ? -1 : user.Credits,
                TotalCost = requiredCredits * 0.04m // $0.04 per credit
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating preview images");
            return StatusCode(500, new { error = "Failed to generate images", details = ex.Message });
        }
    }

    /// <summary>
    /// Generate final high-quality images from approved previews
    /// </summary>
    [HttpPost("generate-final")]
    public async Task<IActionResult> GenerateFinalImages(
        [FromBody] GenerateFinalImagesRequest request,
        CancellationToken cancellationToken)
    {
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

            // Calculate required credits (1 credit per HD image)
            var requiredCredits = request.ImageIds.Count;

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

            _logger.LogInformation("Generating {Count} final images for user {UserId}",
                request.ImageIds.Count, userId);

            // TODO: Load session from database to get original prompts
            // For now, return error asking to regenerate from scratch
            return BadRequest(new
            {
                error = "Session not found. Please regenerate images from the preview step.",
                message = "Final image generation from previews will be available in the next update."
            });

            // Future implementation:
            // 1. Load session and get original prompts for each image ID
            // 2. Regenerate with HD quality
            // 3. Deduct credits
            // 4. Return new HD images
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating final images");
            return StatusCode(500, new { error = "Failed to generate final images", details = ex.Message });
        }
    }

    /// <summary>
    /// Get available image styles
    /// </summary>
    [HttpGet("styles")]
    [AllowAnonymous]
    public IActionResult GetImageStyles()
    {
        var styles = Enum.GetValues<ImageStyle>()
            .Select(s => new
            {
                id = s.ToString(),
                name = GetStyleDisplayName(s),
                description = GetStyleDescription(s),
                example = $"A cute owl sitting on a tree branch, {GetStyleKeywords(s)}"
            })
            .ToList();

        return Ok(styles);
    }

    private string GetStyleDisplayName(ImageStyle style)
    {
        return style switch
        {
            ImageStyle.ThreeD => "3D Art",
            ImageStyle.DigitalArt => "Digital Art",
            ImageStyle.PixelArt => "Pixel Art",
            ImageStyle.VintagePoster => "Vintage Poster",
            _ => style.ToString()
        };
    }

    private string GetStyleDescription(ImageStyle style)
    {
        return style switch
        {
            ImageStyle.Cartoon => "Colorful, playful cartoon style perfect for children's content",
            ImageStyle.Illustration => "Professional illustration ideal for storybooks and educational content",
            ImageStyle.Watercolor => "Soft, artistic watercolor perfect for dreamy, gentle scenes",
            ImageStyle.DigitalArt => "Modern digital painting with vibrant colors and polished finish",
            ImageStyle.PixelArt => "Retro gaming aesthetic with nostalgic 8-bit or 16-bit style",
            ImageStyle.Realistic => "Photorealistic images with accurate details and lighting",
            ImageStyle.Sketch => "Hand-drawn pencil or pen sketches with artistic flair",
            ImageStyle.ThreeD => "3D rendered images with depth, lighting and realistic materials",
            ImageStyle.Anime => "Japanese anime/manga style with expressive characters",
            ImageStyle.Comic => "Bold comic book art with dynamic composition",
            ImageStyle.Minimalist => "Clean, simple design focusing on essential elements",
            ImageStyle.VintagePoster => "Retro poster art with classic 1950s-1970s aesthetic",
            _ => "High-quality artistic style"
        };
    }

    private string GetStyleKeywords(ImageStyle style)
    {
        return style switch
        {
            ImageStyle.Cartoon => "cartoon style, vibrant colors",
            ImageStyle.Illustration => "children's book illustration",
            ImageStyle.Watercolor => "watercolor painting",
            ImageStyle.DigitalArt => "digital art",
            ImageStyle.PixelArt => "pixel art, 16-bit",
            ImageStyle.Realistic => "photorealistic",
            ImageStyle.Sketch => "pencil sketch",
            ImageStyle.ThreeD => "3D rendered, Pixar-style",
            ImageStyle.Anime => "anime style",
            ImageStyle.Comic => "comic book art",
            ImageStyle.Minimalist => "minimalist design",
            ImageStyle.VintagePoster => "vintage poster",
            _ => "artistic"
        };
    }

    /// <summary>
    /// Download an image - proxies the request to Azure Blob Storage with SAS token
    /// </summary>
    [HttpGet("download/{imageId}")]
    public async Task<IActionResult> DownloadImage(string imageId, CancellationToken cancellationToken)
    {
        try
        {
            // Get user ID from JWT claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "Invalid authentication token" });
            }

            _logger.LogInformation("Download request for image {ImageId} by user {UserId}", imageId, userId);

            // Get the blob URL with SAS token from storage service
            var blobUrl = await _storageService.GetImageUrlAsync(userId, imageId, cancellationToken);

            if (string.IsNullOrEmpty(blobUrl))
            {
                return NotFound(new { error = "Image not found" });
            }

            // Fetch the image from blob storage
            var imageBytes = await _storageService.DownloadImageAsync(blobUrl, cancellationToken);

            // Return the image with proper headers
            return File(imageBytes, "image/png", $"icon-{imageId}.png");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading image {ImageId}", imageId);
            return StatusCode(500, new { error = "Failed to download image", details = ex.Message });
        }
    }
}
