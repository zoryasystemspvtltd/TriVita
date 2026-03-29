using Healthcare.Common.Entities;

namespace LMSService.Domain.Entities;

public sealed class LmsTestPackage : BaseEntity
{
    public string PackageCode { get; set; } = null!;
    public string PackageName { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}

public sealed class LmsTestPackageLine : BaseEntity
{
    public long TestPackageId { get; set; }
    public long TestMasterId { get; set; }
    public int LineNum { get; set; }
    public bool IsOptionalInPackage { get; set; }
}

public sealed class LmsTestPrice : BaseEntity
{
    public long? TestMasterId { get; set; }
    public long? TestPackageId { get; set; }
    public long? DepartmentId { get; set; }
    public long? PriceTierReferenceValueId { get; set; }
    public decimal RateAmount { get; set; }
    public string? CurrencyCode { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}

public sealed class LmsReagentMaster : BaseEntity
{
    public string ReagentCode { get; set; } = null!;
    public string ReagentName { get; set; } = null!;
    public long? DefaultUnitId { get; set; }
    public string? StorageNotes { get; set; }
}

public sealed class LmsReagentBatch : BaseEntity
{
    public long ReagentMasterId { get; set; }
    public string LotNo { get; set; } = null!;
    public DateTime? ExpiryDate { get; set; }
    public DateTime? ReceivedOn { get; set; }
    public long? LabInventoryId { get; set; }
    public decimal? OpeningQuantity { get; set; }
    public decimal? CurrentQuantity { get; set; }
}

public sealed class LmsTestReagentMap : BaseEntity
{
    public long TestMasterId { get; set; }
    public long ReagentMasterId { get; set; }
    public decimal QuantityPerTest { get; set; }
    public long? UnitId { get; set; }
}

public sealed class LmsReagentConsumptionLog : BaseEntity
{
    public long ReagentBatchId { get; set; }
    public decimal QuantityConsumed { get; set; }
    public DateTime ConsumedOn { get; set; }
    public long? LabOrderItemId { get; set; }
    public long? WorkQueueId { get; set; }
    public long? ConsumptionReasonReferenceValueId { get; set; }
    public string? Notes { get; set; }
}

public sealed class LmsLabOrderContext : BaseEntity
{
    public long LabOrderId { get; set; }
    public long? B2BPartnerId { get; set; }
    public long? ReferralDoctorProfileId { get; set; }
    public long? SampleSourceReferenceValueId { get; set; }
    public long? BookingChannelReferenceValueId { get; set; }
    public DateTime? ExpectedReportOn { get; set; }
}

public sealed class LmsReportPaymentGate : BaseEntity
{
    public long ReportHeaderId { get; set; }
    public long LabInvoiceHeaderId { get; set; }
    public decimal MinimumPaidPercent { get; set; }
    public bool IsReleased { get; set; }
    public DateTime? ReleasedOn { get; set; }
    public long? ReleaseReasonReferenceValueId { get; set; }
}
