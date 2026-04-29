namespace PharmacyService.Application.DTOs.Entities;

public sealed class CreateMedicineDto
{
    public string MedicineCode { get; set; }
    public string MedicineName { get; set; }
    public long CategoryId { get; set; }
    public long? ManufacturerId { get; set; }
    public string? Strength { get; set; }
    public long? DefaultUnitId { get; set; }
    public long? FormId { get; set; }
    public long? PrimaryCompositionId { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
}