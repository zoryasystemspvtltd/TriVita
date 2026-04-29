namespace PharmacyService.Application.DTOs.Entities;

public sealed class CreateGoodsReceiptItemDto
{
    public long GoodsReceiptId { get; set; }
    public long? PurchaseOrderItemId { get; set; }
    public int LineNum { get; set; }
    public long MedicineId { get; set; }
    public decimal QuantityReceived { get; set; }
    public long? UnitId { get; set; }
    public decimal UnitPrice { get; set; }
    public string BatchNo { get; set; } = null!;
    public DateTime ExpiryDate { get; set; }
    public decimal? MRP { get; set; }
}