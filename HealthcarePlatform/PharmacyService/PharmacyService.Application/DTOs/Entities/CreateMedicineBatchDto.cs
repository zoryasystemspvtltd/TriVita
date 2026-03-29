namespace PharmacyService.Application.DTOs.Entities;

public sealed class CreateMedicineBatchDto
{
    public long MedicineId { get; set; }
    public string BatchNo { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public decimal? MRP { get; set; }
    public decimal? PurchaseRate { get; set; }
    public DateTime? ManufacturingDate { get; set; }
}