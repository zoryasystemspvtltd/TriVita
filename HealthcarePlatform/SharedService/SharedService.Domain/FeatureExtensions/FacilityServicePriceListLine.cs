using SharedService.Domain.Enterprise;

namespace SharedService.Domain.FeatureExtensions;

/// <summary>Maps dbo.EXT_FacilityServicePriceListLine.</summary>
public sealed class FacilityServicePriceListLine : AuditedEntityBase
{
    public long FacilityId { get; set; }

    public long PriceListId { get; set; }

    public string ServiceItemCode { get; set; } = null!;

    public string? ServiceItemName { get; set; }

    public decimal UnitPrice { get; set; }

    public string? TaxCategoryCode { get; set; }
}
