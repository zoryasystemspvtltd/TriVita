namespace LMSService.Application.Options;

public sealed class LmsNotificationIntegrationOptions
{
    public const string SectionName = "LmsNotifications";

    public string CourseAssignedTemplateCode { get; set; } = "LMS_COURSE_ASSIGNED";

    public long StudentRecipientTypeReferenceValueId { get; set; }

    public long EmailChannelReferenceValueId { get; set; }

    public long PriorityNormalReferenceValueId { get; set; }
}
