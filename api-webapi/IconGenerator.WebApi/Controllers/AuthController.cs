namespace IconGenerator.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using IconGenerator.Functions.Models;
using IconGenerator.Functions.Services;
using IconGenerator.WebApi.Services;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IDatabaseService _databaseService;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private const int WELCOME_CREDITS = 2; // Free credits for new users

    public AuthController(
        IDatabaseService databaseService,
        IJwtService jwtService,
        ILogger<AuthController> logger,
        IHttpClientFactory httpClientFactory)
    {
        _databaseService = databaseService;
        _jwtService = jwtService;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Authenticate with Google ID token (popup flow)
    /// </summary>
    [HttpPost("google")]
    public async Task<IActionResult> GoogleAuth([FromBody] GoogleAuthRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Google authentication endpoint triggered");

        try
        {
            if (request == null || string.IsNullOrEmpty(request.IdToken))
            {
                return BadRequest(new { error = "Invalid request body" });
            }

            // Verify Google ID token
            var payload = await VerifyGoogleTokenAsync(request.IdToken);
            if (payload == null)
            {
                return Unauthorized(new { error = "Invalid Google ID token" });
            }

            // Process user authentication
            var result = await ProcessUserAuthenticationAsync(payload, cancellationToken);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Google authentication");
            return StatusCode(500, new { error = "An error occurred during authentication" });
        }
    }

    /// <summary>
    /// Google OAuth callback (redirect flow)
    /// </summary>
    [HttpPost("google/callback")]
    public async Task<IActionResult> GoogleCallback([FromBody] GoogleCallbackRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Google OAuth callback endpoint triggered");

        try
        {
            if (request == null || string.IsNullOrEmpty(request.Code))
            {
                return BadRequest(new { error = "Invalid request body" });
            }

            // Exchange authorization code for tokens
            var googleUser = await ExchangeCodeForTokenAsync(request.Code);
            if (googleUser == null)
            {
                return Unauthorized(new { error = "Failed to authenticate with Google" });
            }

            // Convert GoogleUserInfo to GoogleTokenPayload
            var payload = new GoogleTokenPayload
            {
                Sub = googleUser.Id,
                Email = googleUser.Email,
                Name = googleUser.Name,
                Picture = googleUser.Picture,
                EmailVerified = googleUser.EmailVerified
            };

            // Process user authentication
            var result = await ProcessUserAuthenticationAsync(payload, cancellationToken);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Google OAuth callback");
            return StatusCode(500, new { error = "An error occurred during authentication" });
        }
    }

    #region Private Helper Methods

    private async Task<AuthResponse> ProcessUserAuthenticationAsync(GoogleTokenPayload payload, CancellationToken cancellationToken)
    {
        // Check if user exists
        var existingUser = await _databaseService.GetUserByGoogleIdAsync(payload.Sub, cancellationToken);

        User user;
        bool isNewUser = false;

        if (existingUser == null)
        {
            // Create new user
            user = new User
            {
                Email = payload.Email,
                Name = payload.Name,
                ProfilePictureUrl = payload.Picture,
                Credits = WELCOME_CREDITS,
                Auth = new UserAuth
                {
                    GoogleId = payload.Sub,
                    GoogleEmail = payload.Email,
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

            _logger.LogInformation("New user created: {UserId} ({Email})", user.Id, user.Email);
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
            if (user.Name != payload.Name || user.ProfilePictureUrl != payload.Picture)
            {
                user.Name = payload.Name;
                user.ProfilePictureUrl = payload.Picture;
            }

            await _databaseService.UpdateUserAsync(user, cancellationToken);

            _logger.LogInformation("User logged in: {UserId} ({Email})", user.Id, user.Email);
        }

        // Generate JWT access token
        var accessToken = _jwtService.GenerateToken(user);

        // Return response
        return new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email,
            Name = user.Name,
            ProfilePictureUrl = user.ProfilePictureUrl,
            Credits = user.Credits,
            AccessToken = accessToken,
            IsNewUser = isNewUser
        };
    }

    private async Task<GoogleTokenPayload?> VerifyGoogleTokenAsync(string idToken)
    {
        try
        {
            // In production, verify token with Google's public keys
            // For now, we'll decode the JWT without verification (ONLY FOR DEVELOPMENT)
            // TODO: Add proper Google token verification using Google.Apis.Auth

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(idToken);

            return new GoogleTokenPayload
            {
                Sub = token.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? string.Empty,
                Email = token.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? string.Empty,
                Name = token.Claims.FirstOrDefault(c => c.Type == "name")?.Value,
                Picture = token.Claims.FirstOrDefault(c => c.Type == "picture")?.Value,
                EmailVerified = bool.Parse(token.Claims.FirstOrDefault(c => c.Type == "email_verified")?.Value ?? "false")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying Google token");
            return null;
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
            var handler = new JwtSecurityTokenHandler();

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

    #endregion
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
