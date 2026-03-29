namespace SharedService.Domain.Enterprise;

/// <summary>Maps dbo.Department (FacilityId must equal FacilityParentId per DB constraint).</summary>
public sealed class Department : AuditedEntityBase
{
    /// <summary>FK to Facility.Id — same value as <see cref="FacilityId"/> per CK_Department_Facility_Consistency.</summary>
    public long FacilityParentId { get; set; }

    /// <summary>Facility scope for the department row (NOT NULL).</summary>
    public long FacilityId { get; set; }

    public string DepartmentCode { get; set; } = null!;

    public string DepartmentName { get; set; } = null!;

    public string DepartmentType { get; set; } = null!;

    public long? ParentDepartmentId { get; set; }

    public long? PrimaryAddressId { get; set; }

    public long? PrimaryContactId { get; set; }

    public DateTime? EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }
}
