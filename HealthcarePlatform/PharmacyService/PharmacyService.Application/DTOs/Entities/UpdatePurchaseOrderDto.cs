namespace PharmacyService.Application.DTOs.Entities;

public sealed class UpdatePurchaseOrderDto
{
    public string PurchaseOrderNo { get; set; }
    public string SupplierName { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedOn { get; set; }
    public long StatusReferenceValueId { get; set; }
    public string? Notes { get; set; }

    public decimal DiscountAmount { get; set; }
    public decimal GstPercent { get; set; }
    public decimal OtherTaxAmount { get; set; }
}