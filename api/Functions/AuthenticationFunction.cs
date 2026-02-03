namespace IconGenerator.Functions;

using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using IconGenerator.Functions.Models;
using IconGenerator.Functions.Services;

public class AuthenticationFunction
{
    private readonly IDatabaseService _databaseService;
    private readonly ILogger<AuthenticationFunction> _logger;
    private const int WELCOME_CREDITS = 2; // Free credits for new users

    public AuthenticationFunction(
        IDatabaseService databaseService,
        ILogger<AuthenticationFunction> logger)
    {
        _databaseService = databaseService;
        _logger = logger;
    }

    [Function("GoogleAuth")]
    public async Task<HttpResponseData> GoogleAuth(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth/google")] HttpRequestData req,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Google authentication function triggered");

        try
        {
            // Parse request body
            var request = await req.ReadFromJsonAsync<GoogleAuthRequest>(cancellationToken);
            if (request == null || string.IsNullOrEmpty(request.IdToken))
            {
                var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequestResponse.WriteAsJsonAsync(new { error = "Invalid request body" });
                return badRequestResponse;
            }

            // Verify Google ID token
            var payload = await VerifyGoogleTokenAsync(request.IdToken);
            if (payload == null)
            {
                var unauthorizedResponse = req.CreateResponse(HttpStatusCode.Unauthorized);
                await unauthorizedResponse.WriteAsJsonAsync(new { error = "Invalid Google ID token" });
                return unauthorizedResponse;
            }

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

                _logger.LogInformation($"New user created: {user.Id} ({user.Email})");
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

                _logger.LogInformation($"User logged in: {user.Id} ({user.Email})");
            }

            // Generate access token (simple implementation - in production use JWT with proper signing)
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
            _logger.LogError(ex, "Error during Google authentication");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteAsJsonAsync(new { error = "An error occurred during authentication" });
            return errorResponse;
        }
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

    private string GenerateAccessToken(string userId)
    {
        // Simple base64 encoded token (in production, use proper JWT signing)
        // TODO: Implement proper JWT token generation with signing
        var tokenData = $"{userId}:{DateTime.UtcNow.Ticks}";
        var tokenBytes = System.Text.Encoding.UTF8.GetBytes(tokenData);
        return Convert.ToBase64String(tokenBytes);
    }
}
