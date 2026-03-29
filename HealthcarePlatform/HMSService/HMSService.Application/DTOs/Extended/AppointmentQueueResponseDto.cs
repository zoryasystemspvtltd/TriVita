namespace HMSService.Application.DTOs.Extended;

public sealed class AppointmentQueueResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public long AppointmentId { get; set; }
    public string QueueToken { get; set; }
    public int PositionInQueue { get; set; }
    public long QueueStatusReferenceValueId { get; set; }
    public DateTime EnqueuedOn { get; set; }
    public DateTime? CheckedInOn { get; set; }
    public DateTime? ExpectedServiceOn { get; set; }
}