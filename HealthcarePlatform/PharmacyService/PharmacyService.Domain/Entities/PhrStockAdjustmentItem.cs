using Healthcare.Common.Entities;

namespace PharmacyService.Domain.Entities;

public sealed class PhrStockAdjustmentItem : BaseEntity
{
    public long StockAdjustmentId { get; set; }
    public int LineNum { get; set; }
    public long MedicineBatchId { get; set; }
    public decimal QuantityDelta { get; set; }
    public decimal? UnitCost { get; set; }
    public string? Notes { get; set; }
}