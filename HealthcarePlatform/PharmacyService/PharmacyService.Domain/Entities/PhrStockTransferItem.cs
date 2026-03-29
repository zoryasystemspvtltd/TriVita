using Healthcare.Common.Entities;

namespace PharmacyService.Domain.Entities;

public sealed class PhrStockTransferItem : BaseEntity
{
    public long StockTransferId { get; set; }
    public int LineNum { get; set; }
    public long MedicineBatchId { get; set; }
    public decimal Quantity { get; set; }
    public string? Notes { get; set; }
}