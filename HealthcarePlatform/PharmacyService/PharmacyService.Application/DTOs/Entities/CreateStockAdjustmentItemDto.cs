namespace PharmacyService.Application.DTOs.Entities;

public sealed class CreateStockAdjustmentItemDto
{
    public long StockAdjustmentId { get; set; }
    public int LineNum { get; set; }
    public long MedicineBatchId { get; set; }
    public decimal QuantityDelta { get; set; }
    public decimal? UnitCost { get; set; }
    public string? Notes { get; set; }
}