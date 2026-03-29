namespace SharedService.Domain.Enterprise;

/// <summary>Maps dbo.Enterprise.</summary>
public sealed class EnterpriseRoot : AuditedEntityBase
{
    public long? FacilityId { get; set; }

    public string EnterpriseCode { get; set; } = null!;

    public string EnterpriseName { get; set; } = null!;

    public string? RegistrationDetails { get; set; }

    public string? GlobalSettingsJson { get; set; }

    public long? PrimaryAddressId { get; set; }

    public long? PrimaryContactId { get; set; }

    public DateTime? EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }
}
