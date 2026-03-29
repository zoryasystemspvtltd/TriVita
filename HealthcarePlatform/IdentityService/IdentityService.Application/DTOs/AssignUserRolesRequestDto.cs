namespace IdentityService.Application.DTOs;

public sealed class AssignUserRolesRequestDto
{
    public IReadOnlyList<long> RoleIds { get; set; } = Array.Empty<long>();
}
