namespace HMSService.Application.DTOs.Extended;

public sealed class VitalResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public long VisitId { get; set; }
    public DateTime RecordedOn { get; set; }
    public long VitalReferenceValueId { get; set; }
    public decimal? ValueNumeric { get; set; }
    public decimal? ValueNumeric2 { get; set; }
    public string? ValueText { get; set; }
    public long? UnitId { get; set; }
    public long? RecordedByDoctorId { get; set; }
    public string? Notes { get; set; }
}