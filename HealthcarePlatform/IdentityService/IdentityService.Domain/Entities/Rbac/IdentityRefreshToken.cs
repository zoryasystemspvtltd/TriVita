namespace IdentityService.Domain.Entities.Rbac;

public sealed class IdentityRefreshToken
{
    public long Id { get; set; }

    public long TenantId { get; set; }

    public long? FacilityId { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; }

    public DateTime CreatedOn { get; set; }

    public long CreatedBy { get; set; }

    public DateTime ModifiedOn { get; set; }

    public long ModifiedBy { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public long UserId { get; set; }

    public Guid TokenFamilyId { get; set; }

    public string TokenHash { get; set; } = null!;

    public DateTime ExpiresOn { get; set; }

    public DateTime? RevokedOn { get; set; }

    public long? ReplacedByTokenId { get; set; }

    public string? ClientIp { get; set; }

    public string? UserAgent { get; set; }
}
