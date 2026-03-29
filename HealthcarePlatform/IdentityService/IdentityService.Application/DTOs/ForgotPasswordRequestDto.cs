namespace IdentityService.Application.DTOs;

public sealed class ForgotPasswordRequestDto
{
    public long TenantId { get; set; } = 1;

    public string Email { get; set; } = null!;
}
