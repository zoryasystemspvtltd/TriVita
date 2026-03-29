namespace IdentityService.Application.DTOs;

public sealed class ResetPasswordRequestDto
{
    public long TenantId { get; set; } = 1;

    public string Email { get; set; } = null!;

    public string Token { get; set; } = null!;

    public string NewPassword { get; set; } = null!;
}
