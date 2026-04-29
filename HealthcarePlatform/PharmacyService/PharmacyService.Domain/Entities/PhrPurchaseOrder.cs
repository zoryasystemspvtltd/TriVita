using Healthcare.Common.Entities;

namespace PharmacyService.Domain.Entities;

public sealed class PhrPurchaseOrder : BaseEntity
{
    public string PurchaseOrderNo { get; set; } = null!;
    public string SupplierName { get; set; } = null!;
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedOn { get; set; }
    public long StatusReferenceValueId { get; set; }
    public string? Notes { get; set; }

    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal GstPercent { get; set; }
    public decimal GstAmount { get; set; }
    public decimal OtherTaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
}