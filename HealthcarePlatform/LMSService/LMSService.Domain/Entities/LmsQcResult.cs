using Healthcare.Common.Entities;

namespace LMSService.Domain.Entities;

public sealed class LmsQcResult : BaseEntity
{
    public long QCRecordId { get; set; }
    public long? TestParameterId { get; set; }
    public decimal? ResultNumeric { get; set; }
    public string? ResultText { get; set; }
    public long? ResultUnitId { get; set; }
    public bool IsPass { get; set; }
    public string? Notes { get; set; }
}