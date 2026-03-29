namespace IdentityService.Application.DTOs;

public sealed class TokenResponseDto
{
    public string AccessToken { get; set; } = null!;

    public string TokenType { get; set; } = "Bearer";

    public int ExpiresInSeconds { get; set; }

    public string? RefreshToken { get; set; }

    public int RefreshExpiresInSeconds { get; set; }
}
