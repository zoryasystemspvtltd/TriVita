namespace HMSService.Application.DTOs.Extended;

public sealed class MedicalProcedureResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public long VisitId { get; set; }
    public string ProcedureCode { get; set; }
    public string? ProcedureSystem { get; set; }
    public string? ProcedureDescription { get; set; }
    public DateTime? PerformedOn { get; set; }
    public long? PerformedByDoctorId { get; set; }
    public string? Notes { get; set; }
}