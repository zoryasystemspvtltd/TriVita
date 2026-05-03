using PharmacyService.Domain.Enums;

namespace PharmacyService.Application.DTOs.Entities;

public sealed class PurchaseBillResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public string BillNo { get; set; } = null!;
    public string InvoiceNo { get; set; } = null!;
    public DateTime InvoiceDate { get; set; }
    public long SupplierId { get; set; }
    public long? PurchaseOrderId { get; set; }
    public long GoodsReceiptId { get; set; }
    public PharmacyPurchaseBillSourceMode SourceMode { get; set; }
    public PharmacyPurchaseBillStatus Status { get; set; }
    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal GstPercent { get; set; }
    public decimal GstAmount { get; set; }
    public decimal OtherTaxAmount { get; set; }
    /// <summary>Total payable (aligned with purchase order total).</summary>
    public decimal NetAmount { get; set; }
    public string? Notes { get; set; }
    public List<PurchaseBillItemResponseDto> Items { get; set; } = new();
}
