namespace CommunicationService.Contracts.Notifications;

public sealed class NotificationLogResponseDto
{
    public long Id { get; set; }

    public long NotificationChannelId { get; set; }

    public int AttemptNo { get; set; }

    public string? RequestPayload { get; set; }

    public string? ResponsePayload { get; set; }

    public long StatusReferenceValueId { get; set; }

    public DateTime CreatedOn { get; set; }
}
