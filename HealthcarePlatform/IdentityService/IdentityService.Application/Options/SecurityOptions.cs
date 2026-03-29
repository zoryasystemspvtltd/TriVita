namespace IdentityService.Application.Options;

public sealed class SecurityOptions
{
    public const string SectionName = "Security";

    public int MaxFailedLoginAttempts { get; set; } = 5;

    public int LockoutMinutes { get; set; } = 15;

    public int AccessTokenMinutes { get; set; } = 60;

    public int RefreshTokenDays { get; set; } = 14;

    /// <summary>When set, login for users with MFA accepts this code (development only; do not use in production).</summary>
    public string? MfaDevBypassCode { get; set; }
}
