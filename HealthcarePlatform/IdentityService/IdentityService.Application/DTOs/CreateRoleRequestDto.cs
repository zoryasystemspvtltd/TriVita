namespace IdentityService.Application.DTOs;

public sealed class CreateRoleRequestDto
{
    public long TenantId { get; set; } = 1;

    public string RoleCode { get; set; } = null!;

    public string RoleName { get; set; } = null!;

    public string? Description { get; set; }
}
