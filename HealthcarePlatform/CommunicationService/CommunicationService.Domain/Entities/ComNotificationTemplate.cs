using Healthcare.Common.Entities;

namespace CommunicationService.Domain.Entities;

public sealed class ComNotificationTemplate : BaseEntity
{
    public string TemplateCode { get; set; } = string.Empty;

    public string TemplateName { get; set; } = string.Empty;

    public long ChannelTypeReferenceValueId { get; set; }

    public string? SubjectTemplate { get; set; }

    public string BodyTemplate { get; set; } = string.Empty;

    public int Version { get; set; } = 1;

    public ICollection<ComNotificationChannel> Channels { get; set; } = new List<ComNotificationChannel>();
}
