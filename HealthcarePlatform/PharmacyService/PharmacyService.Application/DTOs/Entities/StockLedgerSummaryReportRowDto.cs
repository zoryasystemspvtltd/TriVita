namespace PharmacyService.Application.DTOs.Entities;

public sealed class StockLedgerSummaryReportRowDto
{
    public string MedicineName { get; set; } = null!;

    public string BatchNumber { get; set; } = null!;

    public DateTime? ExpiryDate { get; set; }

    public decimal OpeningQty { get; set; }

    public decimal TotalIn { get; set; }

    public decimal TotalOut { get; set; }

    public decimal ClosingQty { get; set; }
}
