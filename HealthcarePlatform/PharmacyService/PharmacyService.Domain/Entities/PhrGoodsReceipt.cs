using Healthcare.Common.Entities;

namespace PharmacyService.Domain.Entities;

public sealed class PhrGoodsReceipt : BaseEntity
{
    public string GoodsReceiptNo { get; set; } = null!;
    public long PurchaseOrderId { get; set; }
    public DateTime ReceivedOn { get; set; }
    public long? ReceivedByDoctorId { get; set; }
    public long StatusReferenceValueId { get; set; }
    public string? Notes { get; set; }
}