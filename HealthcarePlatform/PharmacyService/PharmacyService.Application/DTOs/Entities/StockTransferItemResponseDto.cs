namespace PharmacyService.Application.DTOs.Entities;

public sealed class StockTransferItemResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public long StockTransferId { get; set; }
    public int LineNum { get; set; }
    public long MedicineBatchId { get; set; }
    public decimal Quantity { get; set; }
    public string? Notes { get; set; }
}