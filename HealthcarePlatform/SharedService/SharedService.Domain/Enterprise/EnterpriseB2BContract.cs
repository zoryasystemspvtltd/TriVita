namespace SharedService.Domain.Enterprise;

/// <summary>Maps dbo.EXT_EnterpriseB2BContract (corporate / insurance B2B agreements).</summary>
public sealed class EnterpriseB2BContract : AuditedEntityBase
{
    public long? FacilityId { get; set; }

    public long EnterpriseId { get; set; }

    public string PartnerType { get; set; } = null!;

    public string PartnerName { get; set; } = null!;

    public string ContractCode { get; set; } = null!;

    public string? TermsJson { get; set; }

    public DateTime? EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }
}
