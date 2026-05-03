using PharmacyService.Domain.Enums;

namespace PharmacyService.Application.DTOs.Entities;

public sealed class SalesBillResponseDto
{
    public long Id { get; set; }
    public string BillNo { get; set; } = null!;
    public long? CustomerId { get; set; }
    public long? PatientId { get; set; }
    public DateTime SalesDate { get; set; }
    public PharmacySalesBillStatus Status { get; set; }
    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal GstPercent { get; set; }
    public decimal GstAmount { get; set; }
    public decimal OtherTaxAmount { get; set; }
    public decimal NetAmount { get; set; }
    public string? Notes { get; set; }
    public List<SalesBillItemResponseDto> Items { get; set; } = new();
}
