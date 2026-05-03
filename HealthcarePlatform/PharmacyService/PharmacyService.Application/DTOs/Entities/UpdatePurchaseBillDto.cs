namespace PharmacyService.Application.DTOs.Entities;

public sealed class UpdatePurchaseBillDto
{
    public string InvoiceNo { get; set; } = null!;
    public DateTime InvoiceDate { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal GstPercent { get; set; }
    public decimal OtherTaxAmount { get; set; }
    public string? Notes { get; set; }
    public List<PurchaseBillLineInputDto> Items { get; set; } = new();
}
