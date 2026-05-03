namespace PharmacyService.Application.DTOs.Entities;

public sealed class GoodsReceiptPickListDto
{
    public long Id { get; set; }
    public string GoodsReceiptNo { get; set; } = null!;
    public long? PurchaseOrderId { get; set; }
    public long? SupplierId { get; set; }
    public DateTime ReceivedOn { get; set; }
}
