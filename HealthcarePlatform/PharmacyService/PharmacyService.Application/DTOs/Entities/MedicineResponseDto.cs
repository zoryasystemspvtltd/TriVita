namespace PharmacyService.Application.DTOs.Entities;

public sealed class MedicineResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public string MedicineCode { get; set; }
    public string MedicineName { get; set; }
    public long CategoryId { get; set; }
    public long? ManufacturerId { get; set; }
    public string? Strength { get; set; }
    public long? DefaultUnitId { get; set; }
    public long? FormReferenceValueId { get; set; }
    public long? PrimaryCompositionId { get; set; }
    public bool IsActive { get; set; }
    public string? Notes { get; set; }
}