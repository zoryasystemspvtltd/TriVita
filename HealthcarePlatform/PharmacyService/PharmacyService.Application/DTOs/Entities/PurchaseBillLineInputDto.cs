namespace PharmacyService.Application.DTOs.Entities;

public sealed class PurchaseBillLineInputDto
{
    public long GoodsReceiptItemId { get; set; }
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
}
