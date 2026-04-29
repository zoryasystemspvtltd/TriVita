namespace PharmacyService.Application.DTOs.Entities;

public sealed class PurchaseOrderResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public string PurchaseOrderNo { get; set; }
    public string SupplierName { get; set; }
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