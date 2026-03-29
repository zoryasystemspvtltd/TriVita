namespace SharedService.Domain.Enterprise;

/// <summary>Maps dbo.Facility.</summary>
public sealed class Facility : AuditedEntityBase
{
    /// <summary>Optional row-level scope column FacilityId in SQL.</summary>
    public long? FacilityId { get; set; }

    public long BusinessUnitId { get; set; }

    public string FacilityCode { get; set; } = null!;

    public string FacilityName { get; set; } = null!;

    public string FacilityType { get; set; } = null!;

    public string? LicenseDetails { get; set; }

    public string? TimeZoneId { get; set; }

    public string? GeoCode { get; set; }

    public long? PrimaryAddressId { get; set; }

    public long? PrimaryContactId { get; set; }

    public DateTime? EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }
}
