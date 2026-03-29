namespace PharmacyService.Application.DTOs.Entities;

public sealed class MedicineBatchResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public long MedicineId { get; set; }
    public string BatchNo { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public decimal? MRP { get; set; }
    public decimal? PurchaseRate { get; set; }
    public DateTime? ManufacturingDate { get; set; }
}