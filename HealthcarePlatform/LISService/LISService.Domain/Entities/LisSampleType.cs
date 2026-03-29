using Healthcare.Common.Entities;

namespace LISService.Domain.Entities;

public sealed class LisSampleType : BaseEntity
{
    public string SampleTypeCode { get; set; } = null!;
    public string SampleTypeName { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}