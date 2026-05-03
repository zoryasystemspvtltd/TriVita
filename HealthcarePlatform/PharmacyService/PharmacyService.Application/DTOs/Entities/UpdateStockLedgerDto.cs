using PharmacyService.Domain.Enums;

namespace PharmacyService.Application.DTOs.Entities;

public sealed class UpdateStockLedgerDto
{
    public StockLedgerTransactionType TransactionType { get; set; }

    public long ReferenceId { get; set; }

    public long? ReferenceLineId { get; set; }

    public long MedicineId { get; set; }

    public long MedicineBatchId { get; set; }

    public long? LedgerTypeReferenceValueId { get; set; }

    public DateTime TransactionDate { get; set; }

    public decimal QuantityDelta { get; set; }

    public decimal BeforeQty { get; set; }

    public decimal AfterQty { get; set; }

    public decimal? UnitCost { get; set; }

    public decimal? TotalCost { get; set; }

    public string? SourceReference { get; set; }

    public string? Notes { get; set; }
}
