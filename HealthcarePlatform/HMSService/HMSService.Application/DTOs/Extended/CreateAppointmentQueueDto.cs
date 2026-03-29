namespace HMSService.Application.DTOs.Extended;

public sealed class CreateAppointmentQueueDto
{
    public long AppointmentId { get; set; }

    /// <summary>Optional; generated server-side when empty.</summary>
    public string? QueueToken { get; set; }
    public int PositionInQueue { get; set; }
    public long QueueStatusReferenceValueId { get; set; }
    public DateTime EnqueuedOn { get; set; }
    public DateTime? CheckedInOn { get; set; }
    public DateTime? ExpectedServiceOn { get; set; }
}