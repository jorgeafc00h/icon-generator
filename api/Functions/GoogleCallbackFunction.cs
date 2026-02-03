namespace IconGenerator.Functions;

using System.Net;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using IconGenerator.Functions.Models;
using IconGenerator.Functions.Services;

public class GoogleCallbackFunction
{
    private readonly IDatabaseService _databaseService;
    private readonly ILogger<GoogleCallbackFunction> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private const int WELCOME_CREDITS = 2; // Free credits for new users

    public GoogleCallbackFunction(
        IDatabaseService databaseService,
        ILogger<GoogleCallbackFunction> logger,
        IHttpClientFactory httpClientFactory)
    {
        _databaseService = databaseService;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    [Function("GoogleCallback")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth/google/callback")] HttpRequestData req,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Google OAuth callback function triggered");

        try
        {
            // Parse request body
            var request = await req.ReadFromJsonAsync<GoogleCallbackRequest>(cancellationToken);
            if (request == null || string.IsNullOrEmpty(request.Code))
            {
                var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequestResponse.WriteAsJsonAsync(new { error = "Invalid request body" });
                return badRequestResponse;
            }

            // Exchange authorization code for tokens
            var googleUser = await ExchangeCodeForTokenAsync(request.Code);
            if (googleUser == null)
            {
                var unauthorizedResponse = req.CreateResponse(HttpStatusCode.Unauthorized);
                await unauthorizedResponse.WriteAsJsonAsync(new { error = "Failed to authenticate with Google" });
                return unauthorizedResponse;
            }

            // Check if user exists
            var existingUser = await _databaseService.GetUserByGoogleIdAsync(googleUser.Id, cancellationToken);

            User user;
            bool isNewUser = false;

            if (existingUser == null)
            {
                // Create new user
                user = new User
                {
                    Email = googleUser.Email,
                    Name = googleUser.Name,
                    ProfilePictureUrl = googleUser.Picture,
                    Credits = WELCOME_CREDITS,
                    Auth = new UserAuth
                    {
                        GoogleId = googleUser.Id,
                        GoogleEmail = googleUser.Email,
                        LastLoginAt = DateTime.UtcNow
                    },
                    Metadata = new UserMetadata
                    {
                        TotalIconsGenerated = 0,
                        TotalCreditsPurchased = 0,
                        TotalCreditsSpent = 0
                    },
                    Preferences = new UserPreferences()
                };

                await _databaseService.CreateUserAsync(user, cancellationToken);
                isNewUser = true;

                _logger.LogInformation($"New user created via OAuth: {user.Id} ({user.Email})");
            }
            else
            {
                // Update existing user
                user = existingUser;
                user.UpdatedAt = DateTime.UtcNow;
                if (user.Auth != null)
                {
                    user.Auth.LastLoginAt = DateTime.UtcNow;
                }

                // Update profile info if changed
                if (user.Name != googleUser.Name || user.ProfilePictureUrl != googleUser.Picture)
                {
                    user.Name = googleUser.Name;
                    user.ProfilePictureUrl = googleUser.Picture;
                }

                await _databaseService.UpdateUserAsync(user, cancellationToken);

                _logger.LogInformation($"User logged in via OAuth: {user.Id} ({user.Email})");
            }

            // Generate access token
            var accessToken = GenerateAccessToken(user.Id);

            // Return response
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new AuthResponse
            {
                UserId = user.Id,
                Email = user.Email,
                Name = user.Name,
                ProfilePictureUrl = user.ProfilePictureUrl,
                Credits = user.Credits,
                AccessToken = accessToken,
                IsNewUser = isNewUser
            }, cancellationToken);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Google OAuth callback");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteAsJsonAsync(new { error = "An error occurred during authentication" });
            return errorResponse;
        }
    }

    private async Task<GoogleUserInfo?> ExchangeCodeForTokenAsync(string code)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient();

            // For development: Extract user info from the code itself (it's a JWT in popup flow)
            // In production with full OAuth flow, you'd exchange the code for an access token
            // For now, we'll use the ID token approach from the popup flow

            // If using full OAuth redirect flow, you need to:
            // 1. Exchange code for access token with Google's token endpoint
            // 2. Use access token to get user info from Google's userinfo endpoint

            // For simplicity in this implementation, we'll decode the code as a JWT
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();

            // Try to read as JWT (popup flow sends ID token as "code")
            try
            {
                var token = handler.ReadJwtToken(code);

                return new GoogleUserInfo
                {
                    Id = token.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? string.Empty,
                    Email = token.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? string.Empty,
                    Name = token.Claims.FirstOrDefault(c => c.Type == "name")?.Value,
                    Picture = token.Claims.FirstOrDefault(c => c.Type == "picture")?.Value,
                    EmailVerified = bool.Parse(token.Claims.FirstOrDefault(c => c.Type == "email_verified")?.Value ?? "false")
                };
            }
            catch
            {
                // If not a JWT, treat as authorization code and exchange it
                // This would require client secret and token endpoint call
                // For now, return null to indicate failure
                _logger.LogWarning("Code is not a valid JWT token");
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exchanging code for token");
            return null;
        }
    }

    private string GenerateAccessToken(string userId)
    {
        // Simple base64 encoded token (in production, use proper JWT signing)
        var tokenData = $"{userId}:{DateTime.UtcNow.Ticks}";
        var tokenBytes = System.Text.Encoding.UTF8.GetBytes(tokenData);
        return Convert.ToBase64String(tokenBytes);
    }
}

public class GoogleCallbackRequest
{
    public string Code { get; set; } = string.Empty;
}

public class GoogleUserInfo
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Picture { get; set; }
    public bool EmailVerified { get; set; }
}
