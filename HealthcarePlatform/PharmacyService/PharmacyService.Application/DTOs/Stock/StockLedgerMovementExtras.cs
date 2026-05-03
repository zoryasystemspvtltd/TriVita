namespace PharmacyService.Application.DTOs.Stock;

public sealed class StockLedgerMovementExtras
{
    public string? SourceReference { get; init; }

    public long? GrnSupplierId { get; init; }

    public long? SalePatientId { get; init; }

    public long? SaleCustomerId { get; init; }
}
