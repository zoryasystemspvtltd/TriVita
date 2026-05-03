using PharmacyService.Domain.Enums;

namespace PharmacyService.Application.DTOs.Entities;

public sealed class StockLedgerResponseDto
{
    public long Id { get; set; }

    public long TenantId { get; set; }

    public long? FacilityId { get; set; }

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

    public long? GrnSupplierId { get; set; }

    public long? SalePatientId { get; set; }

    public long? SaleCustomerId { get; set; }

    public string? Notes { get; set; }
}
