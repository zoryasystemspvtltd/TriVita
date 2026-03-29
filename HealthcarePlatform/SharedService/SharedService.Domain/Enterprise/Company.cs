namespace SharedService.Domain.Enterprise;

public sealed class Company : AuditedEntityBase
{
    public long? FacilityId { get; set; }

    public long EnterpriseId { get; set; }

    public string CompanyCode { get; set; } = null!;

    public string CompanyName { get; set; } = null!;

    public string? PAN { get; set; }

    public string? GSTIN { get; set; }

    public string? LegalIdentifier1 { get; set; }

    public string? LegalIdentifier2 { get; set; }

    public long? PrimaryAddressId { get; set; }

    public long? PrimaryContactId { get; set; }

    public DateTime? EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }
}
