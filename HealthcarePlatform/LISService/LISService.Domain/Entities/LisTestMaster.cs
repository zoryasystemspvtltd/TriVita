using Healthcare.Common.Entities;

namespace LISService.Domain.Entities;

public sealed class LisTestMaster : BaseEntity
{
    public long CategoryId { get; set; }
    public long? SampleTypeId { get; set; }
    public string TestCode { get; set; } = null!;
    public string TestName { get; set; } = null!;
    public string? TestDescription { get; set; }
    public long? DefaultUnitId { get; set; }
    public bool IsQuantitative { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}