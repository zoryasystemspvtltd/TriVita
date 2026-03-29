namespace SharedService.Application.DTOs.Enterprise;

public sealed class EnterpriseB2BContractResponseDto
{
    public long Id { get; init; }

    public long EnterpriseId { get; init; }

    public long? FacilityId { get; init; }

    public string PartnerType { get; init; } = null!;

    public string PartnerName { get; init; } = null!;

    public string ContractCode { get; init; } = null!;

    public string? TermsJson { get; init; }

    public DateTime? EffectiveFrom { get; init; }

    public DateTime? EffectiveTo { get; init; }

    public bool IsActive { get; init; }
}

public sealed class CreateEnterpriseB2BContractDto
{
    public long EnterpriseId { get; init; }

    public long? FacilityId { get; init; }

    public string PartnerType { get; init; } = null!;

    public string PartnerName { get; init; } = null!;

    public string ContractCode { get; init; } = null!;

    public string? TermsJson { get; init; }

    public DateTime? EffectiveFrom { get; init; }

    public DateTime? EffectiveTo { get; init; }
}

public sealed class UpdateEnterpriseB2BContractDto
{
    public long? FacilityId { get; init; }

    public string PartnerType { get; init; } = null!;

    public string PartnerName { get; init; } = null!;

    public string ContractCode { get; init; } = null!;

    public string? TermsJson { get; init; }

    public DateTime? EffectiveFrom { get; init; }

    public DateTime? EffectiveTo { get; init; }

    public bool IsActive { get; init; } = true;
}
