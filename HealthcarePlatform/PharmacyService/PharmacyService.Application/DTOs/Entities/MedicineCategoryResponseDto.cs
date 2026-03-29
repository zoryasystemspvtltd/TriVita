namespace PharmacyService.Application.DTOs.Entities;

public sealed class MedicineCategoryResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public string CategoryCode { get; set; }
    public string CategoryName { get; set; }
    public string? Description { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}