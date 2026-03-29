namespace IdentityService.Application.DTOs;

public sealed class CreatePermissionRequestDto
{
    public long TenantId { get; set; } = 1;

    public string PermissionCode { get; set; } = null!;

    public string PermissionName { get; set; } = null!;

    public string? ModuleCode { get; set; }

    public string? Description { get; set; }
}
