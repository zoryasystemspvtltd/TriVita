using PharmacyService.Domain.Enums;

namespace PharmacyService.Application.DTOs.Entities;

public sealed class CreatePurchaseBillDto
{
    public PharmacyPurchaseBillSourceMode SourceMode { get; set; }
    public long? PurchaseOrderId { get; set; }
    public long GoodsReceiptId { get; set; }
    public long SupplierId { get; set; }
    public string InvoiceNo { get; set; } = null!;
    public DateTime InvoiceDate { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal GstPercent { get; set; }
    public decimal OtherTaxAmount { get; set; }
    public string? Notes { get; set; }
    /// <summary>When null or empty, lines default from GRN (qty received, purchase rate).</summary>
    public List<PurchaseBillLineInputDto>? Items { get; set; }
}
