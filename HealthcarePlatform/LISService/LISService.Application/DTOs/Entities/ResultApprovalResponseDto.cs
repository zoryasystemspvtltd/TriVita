namespace LISService.Application.DTOs.Entities;

public sealed class ResultApprovalResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public long LabResultsId { get; set; }
    public long ApprovalStatusReferenceValueId { get; set; }
    public long ApprovedByDoctorId { get; set; }
    public DateTime ApprovedOn { get; set; }
    public string? ApprovalNotes { get; set; }
}