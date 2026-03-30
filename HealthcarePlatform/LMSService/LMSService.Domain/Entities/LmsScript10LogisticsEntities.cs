using Healthcare.Common.Entities;

namespace LMSService.Domain.Entities;

/// <summary>Script 10 — LMS_CollectionRequest (home / logistics).</summary>
public sealed class LmsCollectionRequest : BaseEntity
{
    public string RequestNo { get; set; } = null!;
    public long PatientId { get; set; }
    public string? CollectionAddressJson { get; set; }
    public DateTime? RequestedWindowStart { get; set; }
    public DateTime? RequestedWindowEnd { get; set; }
    public long StatusReferenceValueId { get; set; }
    public bool ColdChainRequired { get; set; }
}

public sealed class LmsRiderTracking : BaseEntity
{
    public long CollectionRequestId { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public DateTime RecordedOn { get; set; }
}

public sealed class LmsSampleTransport : BaseEntity
{
    public long CollectionRequestId { get; set; }
    public decimal? TemperatureCelsius { get; set; }
    public DateTime RecordedOn { get; set; }
    public string? Notes { get; set; }
}

/// <summary>FK to LIS lab order enforced in SQL monolith; stored as scalar in split DB.</summary>
public sealed class LmsReportValidationStep : BaseEntity
{
    public long LabOrderId { get; set; }
    public int ValidationLevel { get; set; }
    public long ValidatorUserId { get; set; }
    public long StatusReferenceValueId { get; set; }
    public DateTime? ValidatedOn { get; set; }
    public string? Comments { get; set; }
}

public sealed class LmsResultDeltaCheck : BaseEntity
{
    public long CurrentLabResultId { get; set; }
    public long PriorLabResultId { get; set; }
    public decimal? DeltaPercent { get; set; }
    public bool Flagged { get; set; }
    public DateTime EvaluatedOn { get; set; }
}

public sealed class LmsReportDigitalSign : BaseEntity
{
    public long ReportHeaderId { get; set; }
    public long SignerUserId { get; set; }
    public DateTime SignedOn { get; set; }
    public string? SignatureReference { get; set; }
}
