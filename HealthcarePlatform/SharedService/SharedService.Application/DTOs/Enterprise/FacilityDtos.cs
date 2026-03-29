namespace SharedService.Application.DTOs.Enterprise;

public sealed class FacilityResponseDto
{
    public long Id { get; init; }

    public long BusinessUnitId { get; init; }

    public string FacilityCode { get; init; } = null!;

    public string FacilityName { get; init; } = null!;

    public string FacilityType { get; init; } = null!;

    public string? LicenseDetails { get; init; }

    public string? TimeZoneId { get; init; }

    public string? GeoCode { get; init; }

    public long? PrimaryAddressId { get; init; }

    public long? PrimaryContactId { get; init; }

    public DateTime? EffectiveFrom { get; init; }

    public DateTime? EffectiveTo { get; init; }

    public bool IsActive { get; init; }
}

public sealed class CreateFacilityDto
{
    public long BusinessUnitId { get; init; }

    public string FacilityCode { get; init; } = null!;

    public string FacilityName { get; init; } = null!;

    public string FacilityType { get; init; } = null!;

    public string? LicenseDetails { get; init; }

    public string? TimeZoneId { get; init; }

    public string? GeoCode { get; init; }

    public long? PrimaryAddressId { get; init; }

    public long? PrimaryContactId { get; init; }

    public DateTime? EffectiveFrom { get; init; }

    public DateTime? EffectiveTo { get; init; }
}

public sealed class UpdateFacilityDto
{
    public long BusinessUnitId { get; init; }

    public string FacilityCode { get; init; } = null!;

    public string FacilityName { get; init; } = null!;

    public string FacilityType { get; init; } = null!;

    public string? LicenseDetails { get; init; }

    public string? TimeZoneId { get; init; }

    public string? GeoCode { get; init; }

    public long? PrimaryAddressId { get; init; }

    public long? PrimaryContactId { get; init; }

    public DateTime? EffectiveFrom { get; init; }

    public DateTime? EffectiveTo { get; init; }

    public bool IsActive { get; init; } = true;
}

/// <summary>Returned for facility validation and cross-service hierarchy context.</summary>
public sealed class FacilityHierarchyContextDto
{
    public long TenantId { get; init; }

    public long EnterpriseId { get; init; }

    public long CompanyId { get; init; }

    public long BusinessUnitId { get; init; }

    public long FacilityId { get; init; }

    public string FacilityCode { get; init; } = null!;

    public string FacilityName { get; init; } = null!;

    public string BusinessUnitType { get; init; } = null!;

    public string FacilityType { get; init; } = null!;
}
