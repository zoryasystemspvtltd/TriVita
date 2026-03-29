using Healthcare.Common.Entities;

namespace PharmacyService.Domain.Entities;

public sealed class PhrStockLedger : BaseEntity
{
    public long MedicineBatchId { get; set; }
    public long? LedgerTypeReferenceValueId { get; set; }
    public DateTime TransactionOn { get; set; }
    public decimal QuantityDelta { get; set; }
    public decimal BeforeQty { get; set; }
    public decimal AfterQty { get; set; }
    public decimal? UnitCost { get; set; }
    public decimal? TotalCost { get; set; }
    public string? SourceReference { get; set; }
    public string? Notes { get; set; }
}