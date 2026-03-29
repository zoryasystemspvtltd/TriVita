namespace PharmacyService.Application.DTOs.Entities;

public sealed class UpdateMedicineCompositionDto
{
    public long MedicineId { get; set; }
    public long CompositionId { get; set; }
    public decimal? Amount { get; set; }
    public long? UnitId { get; set; }
}