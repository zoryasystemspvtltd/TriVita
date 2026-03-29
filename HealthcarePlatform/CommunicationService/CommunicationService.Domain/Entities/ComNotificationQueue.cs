using Healthcare.Common.Entities;

namespace CommunicationService.Domain.Entities;

public sealed class ComNotificationQueue : BaseEntity
{
    public long NotificationId { get; set; }

    public ComNotification Notification { get; set; } = null!;

    public DateTime ScheduledOn { get; set; }

    public DateTime? ProcessedOn { get; set; }

    public long StatusReferenceValueId { get; set; }
}
