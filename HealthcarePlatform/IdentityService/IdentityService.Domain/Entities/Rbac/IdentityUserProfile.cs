namespace IdentityService.Domain.Entities.Rbac;

public sealed class IdentityUserProfile
{
    public long UserId { get; set; }

    public long TenantId { get; set; }

    public long? FacilityId { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; }

    public DateTime CreatedOn { get; set; }

    public long CreatedBy { get; set; }

    public DateTime ModifiedOn { get; set; }

    public long ModifiedBy { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public string? DisplayName { get; set; }

    public string? Phone { get; set; }

    public long? PreferredLocaleReferenceValueId { get; set; }

    public string? TimeZoneId { get; set; }

    public string? AvatarUrl { get; set; }

    public bool MfaEnabled { get; set; }
}
