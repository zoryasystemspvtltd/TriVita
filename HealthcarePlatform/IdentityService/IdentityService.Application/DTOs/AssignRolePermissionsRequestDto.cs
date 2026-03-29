namespace IdentityService.Application.DTOs;

public sealed class AssignRolePermissionsRequestDto
{
    public IReadOnlyList<long> PermissionIds { get; set; } = Array.Empty<long>();
}
