using SharedService.Domain.Enterprise;

namespace SharedService.Domain.FeatureExtensions;

/// <summary>Maps dbo.EXT_TenantOnboardingStage.</summary>
public sealed class TenantOnboardingStage : AuditedEntityBase
{
    public long? FacilityId { get; set; }

    public string StageCode { get; set; } = null!;

    public string StageStatus { get; set; } = null!;

    public DateTime? CompletedOn { get; set; }

    public string? MetadataJson { get; set; }
}
