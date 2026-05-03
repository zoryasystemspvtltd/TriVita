using PharmacyService.Domain.Enums;

namespace PharmacyService.Application.DTOs.Entities;

public sealed class StockLedgerSummaryRowDto
{
    public StockLedgerTransactionType TransactionType { get; set; }

    public long MedicineId { get; set; }

    public decimal NetQuantityDelta { get; set; }

    public int EntryCount { get; set; }
}
