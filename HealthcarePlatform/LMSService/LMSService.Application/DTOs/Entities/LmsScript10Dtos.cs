namespace LMSService.Application.DTOs.Entities;

#region Collection request

public sealed class LmsCollectionRequestResponseDto
{
    public long Id { get; init; }
    public long FacilityId { get; init; }
    public string RequestNo { get; init; } = null!;
    public long PatientId { get; init; }
    public string? CollectionAddressJson { get; init; }
    public DateTime? RequestedWindowStart { get; init; }
    public DateTime? RequestedWindowEnd { get; init; }
    public long StatusReferenceValueId { get; init; }
    public bool ColdChainRequired { get; init; }
}

public sealed class CreateLmsCollectionRequestDto
{
    public long PatientId { get; init; }
    public string? CollectionAddressJson { get; init; }
    public DateTime? RequestedWindowStart { get; init; }
    public DateTime? RequestedWindowEnd { get; init; }
    public long StatusReferenceValueId { get; init; }
    public bool ColdChainRequired { get; init; }
}

public sealed class UpdateLmsCollectionRequestDto
{
    public string? CollectionAddressJson { get; init; }
    public DateTime? RequestedWindowStart { get; init; }
    public DateTime? RequestedWindowEnd { get; init; }
    public long StatusReferenceValueId { get; init; }
    public bool ColdChainRequired { get; init; }
    public bool IsActive { get; init; } = true;
}

#endregion

#region Rider / transport

public sealed class LmsRiderTrackingResponseDto
{
    public long Id { get; init; }
    public long CollectionRequestId { get; init; }
    public decimal Latitude { get; init; }
    public decimal Longitude { get; init; }
    public DateTime RecordedOn { get; init; }
}

public sealed class CreateLmsRiderTrackingDto
{
    public long CollectionRequestId { get; init; }
    public decimal Latitude { get; init; }
    public decimal Longitude { get; init; }
    public DateTime RecordedOn { get; init; }
}

public sealed class UpdateLmsRiderTrackingDto
{
    public decimal Latitude { get; init; }
    public decimal Longitude { get; init; }
    public DateTime RecordedOn { get; init; }
    public bool IsActive { get; init; } = true;
}

public sealed class LmsSampleTransportResponseDto
{
    public long Id { get; init; }
    public long CollectionRequestId { get; init; }
    public decimal? TemperatureCelsius { get; init; }
    public DateTime RecordedOn { get; init; }
    public string? Notes { get; init; }
}

public sealed class CreateLmsSampleTransportDto
{
    public long CollectionRequestId { get; init; }
    public decimal? TemperatureCelsius { get; init; }
    public DateTime RecordedOn { get; init; }
    public string? Notes { get; init; }
}

public sealed class UpdateLmsSampleTransportDto
{
    public decimal? TemperatureCelsius { get; init; }
    public DateTime RecordedOn { get; init; }
    public string? Notes { get; init; }
    public bool IsActive { get; init; } = true;
}

#endregion

#region Report workflow

public sealed class LmsReportValidationStepResponseDto
{
    public long Id { get; init; }
    public long LabOrderId { get; init; }
    public int ValidationLevel { get; init; }
    public long ValidatorUserId { get; init; }
    public long StatusReferenceValueId { get; init; }
    public DateTime? ValidatedOn { get; init; }
    public string? Comments { get; init; }
}

public sealed class CreateLmsReportValidationStepDto
{
    public long LabOrderId { get; init; }
    public int ValidationLevel { get; init; }
    public long ValidatorUserId { get; init; }
    public long StatusReferenceValueId { get; init; }
    public DateTime? ValidatedOn { get; init; }
    public string? Comments { get; init; }
}

public sealed class UpdateLmsReportValidationStepDto
{
    public int ValidationLevel { get; init; }
    public long ValidatorUserId { get; init; }
    public long StatusReferenceValueId { get; init; }
    public DateTime? ValidatedOn { get; init; }
    public string? Comments { get; init; }
    public bool IsActive { get; init; } = true;
}

public sealed class LmsResultDeltaCheckResponseDto
{
    public long Id { get; init; }
    public long CurrentLabResultId { get; init; }
    public long PriorLabResultId { get; init; }
    public decimal? DeltaPercent { get; init; }
    public bool Flagged { get; init; }
    public DateTime EvaluatedOn { get; init; }
}

public sealed class CreateLmsResultDeltaCheckDto
{
    public long CurrentLabResultId { get; init; }
    public long PriorLabResultId { get; init; }
    public decimal? DeltaPercent { get; init; }
    public bool Flagged { get; init; }
    public DateTime EvaluatedOn { get; init; }
}

public sealed class UpdateLmsResultDeltaCheckDto
{
    public decimal? DeltaPercent { get; init; }
    public bool Flagged { get; init; }
    public DateTime EvaluatedOn { get; init; }
    public bool IsActive { get; init; } = true;
}

public sealed class LmsReportDigitalSignResponseDto
{
    public long Id { get; init; }
    public long ReportHeaderId { get; init; }
    public long SignerUserId { get; init; }
    public DateTime SignedOn { get; init; }
    public string? SignatureReference { get; init; }
}

public sealed class CreateLmsReportDigitalSignDto
{
    public long ReportHeaderId { get; init; }
    public long SignerUserId { get; init; }
    public DateTime SignedOn { get; init; }
    public string? SignatureReference { get; init; }
}

public sealed class UpdateLmsReportDigitalSignDto
{
    public long SignerUserId { get; init; }
    public DateTime SignedOn { get; init; }
    public string? SignatureReference { get; init; }
    public bool IsActive { get; init; } = true;
}

#endregion
