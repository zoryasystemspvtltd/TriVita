namespace CommunicationService.Contracts.Notifications;

/// <summary>Convenience payload for a single template + channel send.</summary>
public sealed class SendTemplateNotificationRequestDto
{
    public string EventType { get; set; } = string.Empty;

    public long? ReferenceId { get; set; }

    public string TemplateCode { get; set; } = string.Empty;

    public long ChannelTypeReferenceValueId { get; set; }

    public long PriorityReferenceValueId { get; set; }

    public IReadOnlyDictionary<string, string>? Context { get; set; }

    public IReadOnlyList<RecipientInputDto> Recipients { get; set; } = Array.Empty<RecipientInputDto>();

    public DateTime? ScheduledOnUtc { get; set; }
}
