namespace IdentityService.Application.DTOs;

/// <summary>Credentials for <c>POST /auth/token</c>.</summary>
public sealed class LoginRequestDto
{
    public long TenantId { get; set; } = 1;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    /// <summary>Required when MFA is enabled for the account.</summary>
    public string? MfaCode { get; set; }
}
