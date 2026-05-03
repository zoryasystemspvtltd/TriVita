using Healthcare.Common.Entities;
using PharmacyService.Domain.Enums;

namespace PharmacyService.Domain.Entities;

public sealed class PhrPurchaseBill : BaseEntity
{
    public string BillNo { get; set; } = null!;
    public string InvoiceNo { get; set; } = null!;
    public DateTime InvoiceDate { get; set; }
    public long SupplierId { get; set; }
    public long? PurchaseOrderId { get; set; }
    public long GoodsReceiptId { get; set; }
    public PharmacyPurchaseBillSourceMode SourceMode { get; set; }
    public PharmacyPurchaseBillStatus Status { get; set; } = PharmacyPurchaseBillStatus.Draft;

    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal GstPercent { get; set; }
    public decimal GstAmount { get; set; }
    /// <summary>Additional tax (same semantics as <see cref="PhrPurchaseOrder.OtherTaxAmount"/>).</summary>
    public decimal OtherTaxAmount { get; set; }
    /// <summary>Payable total; same formula as PO <see cref="PhrPurchaseOrder.TotalAmount"/>.</summary>
    public decimal NetAmount { get; set; }
    public string? Notes { get; set; }
}
