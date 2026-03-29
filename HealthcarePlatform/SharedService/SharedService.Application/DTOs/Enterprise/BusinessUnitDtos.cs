namespace SharedService.Application.DTOs.Enterprise;

public sealed class BusinessUnitResponseDto
{
    public long Id { get; init; }

    public long CompanyId { get; init; }

    public string BusinessUnitCode { get; init; } = null!;

    public string BusinessUnitName { get; init; } = null!;

    public string BusinessUnitType { get; init; } = null!;

    public string? RegionCode { get; init; }

    public string? CountryCode { get; init; }

    public long? PrimaryAddressId { get; init; }

    public long? PrimaryContactId { get; init; }

    public DateTime? EffectiveFrom { get; init; }

    public DateTime? EffectiveTo { get; init; }

    public bool IsActive { get; init; }
}

public sealed class CreateBusinessUnitDto
{
    public long CompanyId { get; init; }

    public string BusinessUnitCode { get; init; } = null!;

    public string BusinessUnitName { get; init; } = null!;

    public string BusinessUnitType { get; init; } = null!;

    public string? RegionCode { get; init; }

    public string? CountryCode { get; init; }

    public long? PrimaryAddressId { get; init; }

    public long? PrimaryContactId { get; init; }

    public DateTime? EffectiveFrom { get; init; }

    public DateTime? EffectiveTo { get; init; }
}

public sealed class UpdateBusinessUnitDto
{
    public long CompanyId { get; init; }

    public string BusinessUnitCode { get; init; } = null!;

    public string BusinessUnitName { get; init; } = null!;

    public string BusinessUnitType { get; init; } = null!;

    public string? RegionCode { get; init; }

    public string? CountryCode { get; init; }

    public long? PrimaryAddressId { get; init; }

    public long? PrimaryContactId { get; init; }

    public DateTime? EffectiveFrom { get; init; }

    public DateTime? EffectiveTo { get; init; }

    public bool IsActive { get; init; } = true;
}
