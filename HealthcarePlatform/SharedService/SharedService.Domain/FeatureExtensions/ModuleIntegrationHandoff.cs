using SharedService.Domain.Enterprise;

namespace SharedService.Domain.FeatureExtensions;

/// <summary>Maps dbo.EXT_ModuleIntegrationHandoff.</summary>
public sealed class ModuleIntegrationHandoff : AuditedEntityBase
{
    public long? FacilityId { get; set; }

    public string CorrelationId { get; set; } = null!;

    public string SourceModule { get; set; } = null!;

    public string TargetModule { get; set; } = null!;

    public string EntityType { get; set; } = null!;

    public long? SourceEntityId { get; set; }

    public long? TargetEntityId { get; set; }

    public string StatusCode { get; set; } = null!;

    public string? DetailJson { get; set; }
}
