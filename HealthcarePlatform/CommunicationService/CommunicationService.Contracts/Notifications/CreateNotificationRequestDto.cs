namespace CommunicationService.Contracts.Notifications;

/// <summary>Creates a notification intent, recipients, delivery channels, and enqueues for processing.</summary>
public sealed class CreateNotificationRequestDto
{
    public string EventType { get; set; } = string.Empty;

    public long? ReferenceId { get; set; }

    /// <summary>Key-value pairs merged into templates as {{key}} placeholders.</summary>
    public IReadOnlyDictionary<string, string>? Context { get; set; }

    public long PriorityReferenceValueId { get; set; }

    /// <summary>When null, defaults to configured draft/queued status.</summary>
    public long? StatusReferenceValueId { get; set; }

    public IReadOnlyList<RecipientInputDto> Recipients { get; set; } = Array.Empty<RecipientInputDto>();

    public IReadOnlyList<ChannelRequestDto> Channels { get; set; } = Array.Empty<ChannelRequestDto>();

    public DateTime? ScheduledOnUtc { get; set; }
}
