using Healthcare.Common.Entities;

namespace PharmacyService.Domain.Entities;

public sealed class PhrGoodsReceiptItem : BaseEntity
{
    public long GoodsReceiptId { get; set; }
    public long? PurchaseOrderItemId { get; set; }
    public int LineNum { get; set; }
    public long MedicineId { get; set; }
    public long MedicineBatchId { get; set; }
    public string BatchNo { get; set; } = null!;
    public decimal QuantityReceived { get; set; }
    public long? UnitId { get; set; }
    public decimal? PurchaseRate { get; set; }
    public decimal LineTotal { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public decimal? MRP { get; set; }
}