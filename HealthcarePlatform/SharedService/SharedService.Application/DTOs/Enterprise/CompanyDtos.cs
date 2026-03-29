namespace SharedService.Application.DTOs.Enterprise;

public sealed class CompanyResponseDto
{
    public long Id { get; init; }

    public long EnterpriseId { get; init; }

    public string CompanyCode { get; init; } = null!;

    public string CompanyName { get; init; } = null!;

    public string? PAN { get; init; }

    public string? GSTIN { get; init; }

    public string? LegalIdentifier1 { get; init; }

    public string? LegalIdentifier2 { get; init; }

    public long? PrimaryAddressId { get; init; }

    public long? PrimaryContactId { get; init; }

    public DateTime? EffectiveFrom { get; init; }

    public DateTime? EffectiveTo { get; init; }

    public bool IsActive { get; init; }
}

public sealed class CreateCompanyDto
{
    public long EnterpriseId { get; init; }

    public string CompanyCode { get; init; } = null!;

    public string CompanyName { get; init; } = null!;

    public string? PAN { get; init; }

    public string? GSTIN { get; init; }

    public string? LegalIdentifier1 { get; init; }

    public string? LegalIdentifier2 { get; init; }

    public long? PrimaryAddressId { get; init; }

    public long? PrimaryContactId { get; init; }

    public DateTime? EffectiveFrom { get; init; }

    public DateTime? EffectiveTo { get; init; }
}

public sealed class UpdateCompanyDto
{
    public long EnterpriseId { get; init; }

    public string CompanyCode { get; init; } = null!;

    public string CompanyName { get; init; } = null!;

    public string? PAN { get; init; }

    public string? GSTIN { get; init; }

    public string? LegalIdentifier1 { get; init; }

    public string? LegalIdentifier2 { get; init; }

    public long? PrimaryAddressId { get; init; }

    public long? PrimaryContactId { get; init; }

    public DateTime? EffectiveFrom { get; init; }

    public DateTime? EffectiveTo { get; init; }

    public bool IsActive { get; init; } = true;
}
