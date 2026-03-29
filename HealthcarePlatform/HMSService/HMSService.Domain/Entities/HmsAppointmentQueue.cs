using Healthcare.Common.Entities;

namespace HMSService.Domain.Entities;

public sealed class HmsAppointmentQueue : BaseEntity
{
    public long AppointmentId { get; set; }
    public string QueueToken { get; set; } = null!;
    public int PositionInQueue { get; set; }
    public long QueueStatusReferenceValueId { get; set; }
    public DateTime EnqueuedOn { get; set; }
    public DateTime? CheckedInOn { get; set; }
    public DateTime? ExpectedServiceOn { get; set; }
}