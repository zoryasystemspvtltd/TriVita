namespace CommunicationService.Contracts.Notifications;

public sealed class NotificationResponseDto
{
    public long Id { get; set; }

    public long TenantId { get; set; }

    public long FacilityId { get; set; }

    public string EventType { get; set; } = string.Empty;

    public long? ReferenceId { get; set; }

    public long PriorityReferenceValueId { get; set; }

    public long StatusReferenceValueId { get; set; }

    public DateTime CreatedOn { get; set; }

    public int RecipientCount { get; set; }

    public int ChannelCount { get; set; }

    public int QueueCount { get; set; }
}
