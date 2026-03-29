using Healthcare.Common.Entities;

namespace LISService.Domain.Entities;

public sealed class LisTestCategory : BaseEntity
{
    public string CategoryCode { get; set; } = null!;
    public string CategoryName { get; set; } = null!;
    public long? ParentCategoryId { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}