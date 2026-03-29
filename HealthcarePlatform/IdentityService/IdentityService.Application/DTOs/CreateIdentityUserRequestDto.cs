namespace IdentityService.Application.DTOs;

public sealed class CreateIdentityUserRequestDto
{
    public long TenantId { get; set; } = 1;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public long? FacilityId { get; set; }

    /// <summary>Legacy single role label (also add role assignments separately).</summary>
    public string Role { get; set; } = "User";

    public bool IsActive { get; set; } = true;
}
