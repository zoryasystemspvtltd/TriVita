namespace PharmacyService.Application.DTOs.Entities;

public sealed class CreateGoodsReceiptDto
{
    public string GoodsReceiptNo { get; set; }
    public long PurchaseOrderId { get; set; }
    public DateTime ReceivedOn { get; set; }
    public long? ReceivedByDoctorId { get; set; }
    public long StatusReferenceValueId { get; set; }
    public string? Notes { get; set; }
}