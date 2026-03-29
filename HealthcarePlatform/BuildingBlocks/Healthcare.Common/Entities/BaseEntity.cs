namespace Healthcare.Common.Entities;

/// <summary>
/// Standard audit + multi-tenancy fields aligned with SQL schemas (Id PK, TenantId, optional FacilityId, soft delete, rowversion).
/// </summary>
public abstract class BaseEntity
{
    public long Id { get; set; }

    public long TenantId { get; set; }

    /// <summary>Optional for catalog/shared rows; required for facility-scoped transactions in consuming services.</summary>
    public long? FacilityId { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; }

    public DateTime CreatedOn { get; set; }

    public long CreatedBy { get; set; }

    public DateTime ModifiedOn { get; set; }

    public long ModifiedBy { get; set; }

    /// <summary>SQL Server rowversion / timestamp for optimistic concurrency.</summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
