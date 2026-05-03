using Healthcare.Common.Pagination;
using PharmacyService.Domain.Enums;

namespace PharmacyService.Application.DTOs.Entities;

public sealed class StockLedgerReportQuery : PagedQuery
{
    public StockLedgerTransactionType? TransactionType { get; set; }

    public long? MedicineId { get; set; }

    public long? MedicineBatchId { get; set; }

    public DateTime? FromTransactionDate { get; set; }

    public DateTime? ToTransactionDate { get; set; }
}
