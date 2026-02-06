namespace IconGenerator.WebApi.Options;

/// <summary>
/// Configuration options for JWT token generation and validation
/// </summary>
public class JwtOptions
{
    public const string SectionName = "Jwt";

    /// <summary>
    /// Secret key for signing JWT tokens (minimum 256 bits / 32 bytes)
    /// Generate with: openssl rand -base64 32
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Token issuer (who creates the token)
    /// </summary>
    public string Issuer { get; set; } = "icon-generator-api";

    /// <summary>
    /// Token audience (who can use the token)
    /// </summary>
    public string Audience { get; set; } = "icon-generator-client";

    /// <summary>
    /// Token expiration time in minutes (default: 7 days = 10080 minutes)
    /// </summary>
    public int ExpirationMinutes { get; set; } = 10080;
}
