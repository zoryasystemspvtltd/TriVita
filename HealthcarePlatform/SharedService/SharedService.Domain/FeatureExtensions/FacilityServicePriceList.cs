using SharedService.Domain.Enterprise;

namespace SharedService.Domain.FeatureExtensions;

/// <summary>Maps dbo.EXT_FacilityServicePriceList.</summary>
public sealed class FacilityServicePriceList : AuditedEntityBase
{
    public long FacilityId { get; set; }

    public string PriceListCode { get; set; } = null!;

    public string PriceListName { get; set; } = null!;

    public string ServiceModule { get; set; } = null!;

    public string? PartnerReferenceCode { get; set; }

    public string CurrencyCode { get; set; } = "INR";

    public DateTime? EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }
}
