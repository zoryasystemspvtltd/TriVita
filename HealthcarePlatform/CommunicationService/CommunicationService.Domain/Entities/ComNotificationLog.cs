using Healthcare.Common.Entities;

namespace CommunicationService.Domain.Entities;

public sealed class ComNotificationLog : BaseEntity
{
    public long NotificationChannelId { get; set; }

    public ComNotificationChannel NotificationChannel { get; set; } = null!;

    public int AttemptNo { get; set; }

    public string? RequestPayload { get; set; }

    public string? ResponsePayload { get; set; }

    public long StatusReferenceValueId { get; set; }
}
