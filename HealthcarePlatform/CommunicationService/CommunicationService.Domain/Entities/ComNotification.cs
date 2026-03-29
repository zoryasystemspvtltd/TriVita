using Healthcare.Common.Entities;

namespace CommunicationService.Domain.Entities;

public sealed class ComNotification : BaseEntity
{
    public string EventType { get; set; } = string.Empty;

    public long? ReferenceId { get; set; }

    /// <summary>JSON dictionary for template placeholders (serialized at API time).</summary>
    public string? ContextJson { get; set; }

    public long PriorityReferenceValueId { get; set; }

    public long StatusReferenceValueId { get; set; }

    public ICollection<ComNotificationRecipient> Recipients { get; set; } = new List<ComNotificationRecipient>();

    public ICollection<ComNotificationChannel> Channels { get; set; } = new List<ComNotificationChannel>();

    public ICollection<ComNotificationQueue> Queues { get; set; } = new List<ComNotificationQueue>();
}
