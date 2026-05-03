using Healthcare.Common.Pagination;
using PharmacyService.Domain.Enums;

namespace PharmacyService.Application.DTOs.Entities;

public sealed class StockLedgerReportQuery : PagedQuery
{
    public DateTime? FromTransactionDate { get; set; }

    public DateTime? ToTransactionDate { get; set; }

    public StockLedgerTransactionType? TransactionType { get; set; }

    public long? MedicineId { get; set; }

    public long? MedicineBatchId { get; set; }

    public long? SupplierId { get; set; }

    /// <summary>Matches ledger sale rows where patient or customer id equals this value.</summary>
    public long? SalePartyId { get; set; }

    /// <summary>When set, filters ledger rows for this facility; otherwise uses tenant facility from context.</summary>
    public long? FacilityId { get; set; }
}
