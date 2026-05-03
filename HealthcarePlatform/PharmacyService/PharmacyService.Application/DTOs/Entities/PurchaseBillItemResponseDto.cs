namespace PharmacyService.Application.DTOs.Entities;

public sealed class PurchaseBillItemResponseDto
{
    public long Id { get; set; }
    public long PurchaseBillId { get; set; }
    public long GoodsReceiptId { get; set; }
    public long GoodsReceiptItemId { get; set; }
    public int LineNum { get; set; }
    public long MedicineId { get; set; }
    public string MedicineName { get; set; } = null!;
    public long MedicineBatchId { get; set; }
    public string BatchNo { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }
}
