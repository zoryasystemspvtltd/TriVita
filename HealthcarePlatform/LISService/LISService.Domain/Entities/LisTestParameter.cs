using Healthcare.Common.Entities;

namespace LISService.Domain.Entities;

public sealed class LisTestParameter : BaseEntity
{
    public long TestMasterId { get; set; }
    public string ParameterCode { get; set; } = null!;
    public string ParameterName { get; set; } = null!;
    public int DisplayOrder { get; set; }
    public bool IsNumeric { get; set; }
    public long? UnitId { get; set; }
    public string? ParameterNotes { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}