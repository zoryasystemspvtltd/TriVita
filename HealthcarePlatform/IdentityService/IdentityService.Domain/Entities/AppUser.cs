namespace IdentityService.Domain.Entities;

/// <summary>Application user for authentication (align with your enterprise user store).</summary>
public sealed class AppUser
{
    public long Id { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public long TenantId { get; set; }

    public long? FacilityId { get; set; }

    public string Role { get; set; } = null!;

    public bool IsActive { get; set; } = true;
}
