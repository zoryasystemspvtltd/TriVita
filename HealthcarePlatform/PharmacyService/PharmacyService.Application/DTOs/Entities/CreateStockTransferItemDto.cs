namespace PharmacyService.Application.DTOs.Entities;

public sealed class CreateStockTransferItemDto
{
    public long StockTransferId { get; set; }
    public int LineNum { get; set; }
    public long MedicineBatchId { get; set; }
    public decimal Quantity { get; set; }
    public string? Notes { get; set; }
}