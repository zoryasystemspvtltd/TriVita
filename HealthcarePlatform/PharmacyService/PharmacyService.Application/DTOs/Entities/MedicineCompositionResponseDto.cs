namespace PharmacyService.Application.DTOs.Entities;

public sealed class MedicineCompositionResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public long MedicineId { get; set; }
    public long CompositionId { get; set; }
    public decimal? Amount { get; set; }
    public long? UnitId { get; set; }
}