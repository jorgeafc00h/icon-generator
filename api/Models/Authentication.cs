namespace IconGenerator.Functions.Models;

public class GoogleAuthRequest
{
    public string IdToken { get; set; } = string.Empty;
}

public class AuthResponse
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public int Credits { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public bool IsNewUser { get; set; }
}

public class GoogleTokenPayload
{
    public string Sub { get; set; } = string.Empty; // Google User ID
    public string Email { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Picture { get; set; }
    public bool EmailVerified { get; set; }
}
