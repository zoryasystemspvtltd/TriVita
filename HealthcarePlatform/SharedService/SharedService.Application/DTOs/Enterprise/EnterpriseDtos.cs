namespace SharedService.Application.DTOs.Enterprise;

public sealed class EnterpriseResponseDto
{
    public long Id { get; init; }

    public string EnterpriseCode { get; init; } = null!;

    public string EnterpriseName { get; init; } = null!;

    public string? RegistrationDetails { get; init; }

    public string? GlobalSettingsJson { get; init; }

    public long? PrimaryAddressId { get; init; }

    public long? PrimaryContactId { get; init; }

    public DateTime? EffectiveFrom { get; init; }

    public DateTime? EffectiveTo { get; init; }

    public bool IsActive { get; init; }
}

public sealed class CreateEnterpriseDto
{
    public string EnterpriseCode { get; init; } = null!;

    public string EnterpriseName { get; init; } = null!;

    public string? RegistrationDetails { get; init; }

    public string? GlobalSettingsJson { get; init; }

    public long? PrimaryAddressId { get; init; }

    public long? PrimaryContactId { get; init; }

    public DateTime? EffectiveFrom { get; init; }

    public DateTime? EffectiveTo { get; init; }
}

public sealed class UpdateEnterpriseDto
{
    public string EnterpriseCode { get; init; } = null!;

    public string EnterpriseName { get; init; } = null!;

    public string? RegistrationDetails { get; init; }

    public string? GlobalSettingsJson { get; init; }

    public long? PrimaryAddressId { get; init; }

    public long? PrimaryContactId { get; init; }

    public DateTime? EffectiveFrom { get; init; }

    public DateTime? EffectiveTo { get; init; }

    public bool IsActive { get; init; } = true;
}
