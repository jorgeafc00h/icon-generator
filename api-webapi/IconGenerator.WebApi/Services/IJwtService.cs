namespace IconGenerator.WebApi.Services;

using IconGenerator.Functions.Models;
using System.Security.Claims;

/// <summary>
/// Service for generating and validating JWT tokens
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Generate a JWT token for the given user
    /// </summary>
    /// <param name="user">User to generate token for</param>
    /// <returns>JWT token string</returns>
    string GenerateToken(User user);

    /// <summary>
    /// Validate a JWT token and extract claims
    /// </summary>
    /// <param name="token">JWT token to validate</param>
    /// <returns>ClaimsPrincipal if valid, null if invalid</returns>
    ClaimsPrincipal? ValidateToken(string token);

    /// <summary>
    /// Extract user ID from token
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>User ID if valid, null if invalid</returns>
    string? GetUserIdFromToken(string token);
}
