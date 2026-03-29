namespace PharmacyService.Application.DTOs.Entities;

public sealed class GoodsReceiptItemResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public long GoodsReceiptId { get; set; }
    public long PurchaseOrderItemId { get; set; }
    public int LineNum { get; set; }
    public long MedicineId { get; set; }
    public long MedicineBatchId { get; set; }
    public decimal QuantityReceived { get; set; }
    public long? UnitId { get; set; }
    public decimal? PurchaseRate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public decimal? MRP { get; set; }
}