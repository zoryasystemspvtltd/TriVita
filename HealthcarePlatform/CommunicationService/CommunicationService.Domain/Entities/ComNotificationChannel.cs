using Healthcare.Common.Entities;

namespace CommunicationService.Domain.Entities;

public sealed class ComNotificationChannel : BaseEntity
{
    public long NotificationId { get; set; }

    public ComNotification Notification { get; set; } = null!;

    public long ChannelTypeReferenceValueId { get; set; }

    public long? TemplateId { get; set; }

    public ComNotificationTemplate? Template { get; set; }

    public long StatusReferenceValueId { get; set; }

    public int AttemptCount { get; set; }

    public DateTime? LastAttemptOn { get; set; }

    public DateTime? SentOn { get; set; }

    public string? ErrorMessage { get; set; }

    public ICollection<ComNotificationLog> Logs { get; set; } = new List<ComNotificationLog>();
}
