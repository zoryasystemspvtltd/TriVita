namespace PharmacyService.Application.DTOs.Entities;

public sealed class SalesBillLineInputDto
{
    public long MedicineId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
