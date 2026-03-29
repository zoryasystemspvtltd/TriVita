using SharedService.Domain.Enterprise;

namespace SharedService.Domain.FeatureExtensions;

/// <summary>Maps dbo.EXT_CrossFacilityReportAudit.</summary>
public sealed class CrossFacilityReportAudit : AuditedEntityBase
{
    public long? FacilityId { get; set; }

    public string ReportCode { get; set; } = null!;

    public string? ReportName { get; set; }

    public string? FacilityScopeJson { get; set; }

    public string? FilterJson { get; set; }

    public int? ResultRowCount { get; set; }

    public DateTime? CompletedOn { get; set; }
}
