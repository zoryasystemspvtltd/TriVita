using Healthcare.Common.Entities;

namespace LMSService.Domain.Entities;

public sealed class LmsTechnicianAssignment : BaseEntity
{
    public long WorkQueueId { get; set; }
    public long TechnicianDoctorId { get; set; }
    public long AssignmentStatusReferenceValueId { get; set; }
    public DateTime AssignedOn { get; set; }
    public DateTime? ReleasedOn { get; set; }
    public string? Notes { get; set; }
}