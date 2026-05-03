namespace PharmacyService.Application.DTOs.Entities;

public sealed class StockLedgerDetailedRowDto
{
    public DateTime TransactionDate { get; set; }

    public string TransactionType { get; set; } = null!;

    public string ReferenceNo { get; set; } = null!;

    public string MedicineName { get; set; } = null!;

    public string BatchNumber { get; set; } = null!;

    public DateTime? ExpiryDate { get; set; }

    public decimal QuantityIn { get; set; }

    public decimal QuantityOut { get; set; }

    public decimal Balance { get; set; }
}
