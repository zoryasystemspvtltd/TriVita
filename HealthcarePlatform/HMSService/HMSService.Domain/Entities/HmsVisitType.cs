using Healthcare.Common.Entities;

namespace HMSService.Domain.Entities;

/// <summary>Catalog: OPD/ER/follow-up visit types.</summary>
public sealed class HmsVisitType : BaseEntity
{
    public string VisitTypeCode { get; set; } = null!;

    public string VisitTypeName { get; set; } = null!;

    public DateTime? EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }
}
