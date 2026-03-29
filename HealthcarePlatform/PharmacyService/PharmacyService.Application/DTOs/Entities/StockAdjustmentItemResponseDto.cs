namespace PharmacyService.Application.DTOs.Entities;

public sealed class StockAdjustmentItemResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public long StockAdjustmentId { get; set; }
    public int LineNum { get; set; }
    public long MedicineBatchId { get; set; }
    public decimal QuantityDelta { get; set; }
    public decimal? UnitCost { get; set; }
    public string? Notes { get; set; }
}