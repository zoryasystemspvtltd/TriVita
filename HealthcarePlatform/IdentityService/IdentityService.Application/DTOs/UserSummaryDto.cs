namespace IdentityService.Application.DTOs;

public sealed class UserSummaryDto
{
    public long UserId { get; set; }

    public string Email { get; set; } = null!;

    public long TenantId { get; set; }

    public long? FacilityId { get; set; }

    public string Role { get; set; } = null!;

    public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();

    public IReadOnlyList<string> Permissions { get; set; } = Array.Empty<string>();
}
