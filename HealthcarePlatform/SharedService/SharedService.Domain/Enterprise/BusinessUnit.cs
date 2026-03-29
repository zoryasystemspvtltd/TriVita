namespace SharedService.Domain.Enterprise;

public sealed class BusinessUnit : AuditedEntityBase
{
    public long? FacilityId { get; set; }

    public long CompanyId { get; set; }

    public string BusinessUnitCode { get; set; } = null!;

    public string BusinessUnitName { get; set; } = null!;

    public string BusinessUnitType { get; set; } = null!;

    public string? RegionCode { get; set; }

    public string? CountryCode { get; set; }

    public long? PrimaryAddressId { get; set; }

    public long? PrimaryContactId { get; set; }

    public DateTime? EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }
}
