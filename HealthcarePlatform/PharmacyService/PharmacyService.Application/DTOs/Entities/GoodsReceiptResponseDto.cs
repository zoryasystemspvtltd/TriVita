namespace PharmacyService.Application.DTOs.Entities;

public sealed class GoodsReceiptResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public string GoodsReceiptNo { get; set; }
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