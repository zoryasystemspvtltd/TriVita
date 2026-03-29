namespace HMSService.Application.DTOs.Extended;

public sealed class DiagnosisResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public long VisitId { get; set; }
    public long? DiagnosisTypeReferenceValueId { get; set; }
    public string ICDSystem { get; set; }
    public string ICDCode { get; set; }
    public string? ICDVersion { get; set; }
    public string? ICDDescription { get; set; }
    public DateTime? DiagnosisOn { get; set; }
}