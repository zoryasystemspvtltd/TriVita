namespace LISService.Application.Options;

/// <summary>Maps LIS flows to CommunicationService reference data (configure per tenant seed).</summary>
public sealed class LisNotificationIntegrationOptions
{
    public const string SectionName = "LisNotifications";

    public string LabReportReadyTemplateCode { get; set; } = "LAB_REPORT_READY";

    public long PatientRecipientTypeReferenceValueId { get; set; }

    public long EmailChannelReferenceValueId { get; set; }

    public long PriorityNormalReferenceValueId { get; set; }
}
