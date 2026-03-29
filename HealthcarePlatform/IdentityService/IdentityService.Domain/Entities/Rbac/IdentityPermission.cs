namespace IdentityService.Domain.Entities.Rbac;

public sealed class IdentityPermission
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

    public string PermissionCode { get; set; } = null!;

    public string PermissionName { get; set; } = null!;

    public string? ModuleCode { get; set; }

    public string? Description { get; set; }
}
