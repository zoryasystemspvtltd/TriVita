using Healthcare.Common.Entities;

namespace LISService.Domain.Entities;

public sealed class LisResultApproval : BaseEntity
{
    public long LabResultsId { get; set; }
    public long ApprovalStatusReferenceValueId { get; set; }
    public long ApprovedByDoctorId { get; set; }
    public DateTime ApprovedOn { get; set; }
    public string? ApprovalNotes { get; set; }
}