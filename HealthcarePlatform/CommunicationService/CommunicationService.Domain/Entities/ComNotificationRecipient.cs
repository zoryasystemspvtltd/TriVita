using Healthcare.Common.Entities;

namespace CommunicationService.Domain.Entities;

public sealed class ComNotificationRecipient : BaseEntity
{
    public long NotificationId { get; set; }

    public ComNotification Notification { get; set; } = null!;

    public long RecipientTypeReferenceValueId { get; set; }

    public long? RecipientId { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public bool IsPrimary { get; set; }
}
