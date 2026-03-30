using Healthcare.Common.Entities;

namespace LISService.Domain.Entities;

public sealed class LisAnalyzerResultHeader : BaseEntity
{
    public string BarcodeValue { get; set; } = null!;
    public long LmsTestBookingItemId { get; set; }
    public long LmsCatalogTestId { get; set; }
    public long? EquipmentId { get; set; }
    public string? EquipmentAssayCode { get; set; }
    public DateTime ReceivedOn { get; set; }
    public bool TechnicallyVerified { get; set; }
    public DateTime? TechnicallyVerifiedOn { get; set; }
    public bool ReadyForDispatch { get; set; }
    public long ResultStatusReferenceValueId { get; set; }
}

public sealed class LisAnalyzerResultLine : BaseEntity
{
    public long AnalyzerResultHeaderId { get; set; }
    public long? LmsCatalogParameterId { get; set; }
    public string? EquipmentResultCode { get; set; }
    public decimal? ResultNumeric { get; set; }
    public string? ResultText { get; set; }
    public long? ResultUnitId { get; set; }
    public long LineStatusReferenceValueId { get; set; }
}
