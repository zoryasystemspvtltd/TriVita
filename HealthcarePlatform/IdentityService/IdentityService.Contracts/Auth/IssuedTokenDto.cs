namespace IdentityService.Contracts.Auth;

/// <summary>Contract DTO for token responses shared with API gateways/clients.</summary>
public sealed class IssuedTokenDto
{
    public string AccessToken { get; set; } = null!;

    public string TokenType { get; set; } = "Bearer";

    public int ExpiresInSeconds { get; set; }
}
