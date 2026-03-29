namespace LISService.Application.DTOs.Entities;

public sealed class UpdateResultApprovalDto
{
    public long LabResultsId { get; set; }
    public long ApprovalStatusReferenceValueId { get; set; }
    public long ApprovedByDoctorId { get; set; }
    public DateTime ApprovedOn { get; set; }
    public string? ApprovalNotes { get; set; }
}