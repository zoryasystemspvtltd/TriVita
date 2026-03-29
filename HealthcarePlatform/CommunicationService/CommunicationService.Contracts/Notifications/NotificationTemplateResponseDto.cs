namespace CommunicationService.Contracts.Notifications;

public sealed class NotificationTemplateResponseDto
{
    public long Id { get; set; }

    public string TemplateCode { get; set; } = string.Empty;

    public string TemplateName { get; set; } = string.Empty;

    public long ChannelTypeReferenceValueId { get; set; }

    public int Version { get; set; }

    public bool IsActive { get; set; }
}
