using SharedService.Domain.Enterprise;

namespace SharedService.Domain.FeatureExtensions;

/// <summary>Maps dbo.EXT_LabCriticalValueEscalation.</summary>
public sealed class LabCriticalValueEscalation : AuditedEntityBase
{
    public long FacilityId { get; set; }

    public long? LabOrderId { get; set; }

    public long? LabOrderItemId { get; set; }

    public long? LabResultId { get; set; }

    public int EscalationLevel { get; set; } = 1;

    public string ChannelCode { get; set; } = null!;

    public string? RecipientSummary { get; set; }

    public DateTime? DispatchedOn { get; set; }

    public DateTime? AcknowledgedOn { get; set; }

    public string? OutcomeCode { get; set; }
}
