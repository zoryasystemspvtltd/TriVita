using Healthcare.Common.Entities;

namespace PharmacyService.Domain.Entities;

public sealed class PhrPurchaseBillItem : BaseEntity
{
    public long PurchaseBillId { get; set; }
    public long GoodsReceiptId { get; set; }
    public long GoodsReceiptItemId { get; set; }
    public int LineNum { get; set; }
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }
}
