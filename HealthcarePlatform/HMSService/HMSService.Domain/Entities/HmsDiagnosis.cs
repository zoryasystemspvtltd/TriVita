using Healthcare.Common.Entities;

namespace HMSService.Domain.Entities;

public sealed class HmsDiagnosis : BaseEntity
{
    public long VisitId { get; set; }
    public long? DiagnosisTypeReferenceValueId { get; set; }
    public string ICDSystem { get; set; } = null!;
    public string ICDCode { get; set; } = null!;
    public string? ICDVersion { get; set; }
    public string? ICDDescription { get; set; }
    public DateTime? DiagnosisOn { get; set; }
}