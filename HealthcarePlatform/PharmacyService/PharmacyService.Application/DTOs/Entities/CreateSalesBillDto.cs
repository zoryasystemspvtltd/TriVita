namespace PharmacyService.Application.DTOs.Entities;

public sealed class CreateSalesBillDto
{
    public long? CustomerId { get; set; }
    public long? PatientId { get; set; }
    public DateTime SalesDate { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal GstPercent { get; set; }
    public decimal OtherTaxAmount { get; set; }
    public string? Notes { get; set; }
    public List<SalesBillLineInputDto> Items { get; set; } = new();
}
