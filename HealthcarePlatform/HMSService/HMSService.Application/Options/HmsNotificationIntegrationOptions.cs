namespace HMSService.Application.Options;

/// <summary>Maps HMS flows to CommunicationService reference data (configure per tenant seed).</summary>
public sealed class HmsNotificationIntegrationOptions
{
    public const string SectionName = "HmsNotifications";

    public string AppointmentCreatedTemplateCode { get; set; } = "APPT_CONFIRM";

    public long PatientRecipientTypeReferenceValueId { get; set; }

    public long EmailChannelReferenceValueId { get; set; }

    public long PriorityNormalReferenceValueId { get; set; }
}
