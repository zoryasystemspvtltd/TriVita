namespace SharedService.Domain.Enterprise;

/// <summary>Common audit columns aligned with 00_EnterpriseHierarchy_Root.sql (excludes per-table FacilityId).</summary>
public abstract class AuditedEntityBase
{
    public long Id { get; set; }

    public long TenantId { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; }

    public DateTime CreatedOn { get; set; }

    public long CreatedBy { get; set; }

    public DateTime ModifiedOn { get; set; }

    public long ModifiedBy { get; set; }

    /// <summary>Concurrency token; null on insert until loaded from database.</summary>
    public byte[]? RowVersion { get; set; }
}
