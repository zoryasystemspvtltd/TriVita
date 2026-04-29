using Healthcare.Common.Entities;

namespace PharmacyService.Domain.Entities;

public sealed class PhrGoodsReceipt : BaseEntity
{
    public string GoodsReceiptNo { get; set; } = null!;
    public long? PurchaseOrderId { get; set; }
    public long? SupplierId { get; set; }
    public DateTime ReceivedOn { get; set; }
    public long? ReceivedByDoctorId { get; set; }
    public long StatusReferenceValueId { get; set; }
    public string? Notes { get; set; }

    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal GstPercent { get; set; }
    public decimal GstAmount { get; set; }
    public decimal OtherTaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
}