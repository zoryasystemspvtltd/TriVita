namespace SharedService.Domain.Enterprise;

public sealed class ContactDetails : AuditedEntityBase
{
    public long? FacilityId { get; set; }

    public string ContactType { get; set; } = null!;

    public string ContactValue { get; set; } = null!;

    public string? CountryCode { get; set; }

    public string? Extension { get; set; }

    public bool IsPrimary { get; set; }

    public DateTime? EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }
}
