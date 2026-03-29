namespace PharmacyService.Application.Options;

public sealed class PharmacyNotificationIntegrationOptions
{
    public const string SectionName = "PharmacyNotifications";

    public string PrescriptionCreatedTemplateCode { get; set; } = "RX_CREATED";

    public long PatientRecipientTypeReferenceValueId { get; set; }

    public long EmailChannelReferenceValueId { get; set; }

    public long PriorityNormalReferenceValueId { get; set; }
}
