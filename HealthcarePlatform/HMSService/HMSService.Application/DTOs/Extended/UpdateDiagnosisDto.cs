namespace HMSService.Application.DTOs.Extended;

public sealed class UpdateDiagnosisDto
{
    public long VisitId { get; set; }
    public long? DiagnosisTypeReferenceValueId { get; set; }
    public string ICDSystem { get; set; }
    public string ICDCode { get; set; }
    public string? ICDVersion { get; set; }
    public string? ICDDescription { get; set; }
    public DateTime? DiagnosisOn { get; set; }
}