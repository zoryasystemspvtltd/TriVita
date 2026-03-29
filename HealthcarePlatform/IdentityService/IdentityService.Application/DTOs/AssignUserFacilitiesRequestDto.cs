namespace IdentityService.Application.DTOs;

public sealed class AssignUserFacilitiesRequestDto
{
    public IReadOnlyList<long> GrantFacilityIds { get; set; } = Array.Empty<long>();
}
