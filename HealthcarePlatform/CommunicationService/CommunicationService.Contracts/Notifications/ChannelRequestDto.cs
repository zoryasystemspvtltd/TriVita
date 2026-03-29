namespace CommunicationService.Contracts.Notifications;

public sealed class ChannelRequestDto
{
    public long ChannelTypeReferenceValueId { get; set; }

    /// <summary>Optional template code; when set, subject/body are resolved from COM_NotificationTemplate.</summary>
    public string? TemplateCode { get; set; }
}
