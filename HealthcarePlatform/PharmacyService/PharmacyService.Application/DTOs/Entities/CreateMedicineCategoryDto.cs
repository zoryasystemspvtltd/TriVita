namespace PharmacyService.Application.DTOs.Entities;

public sealed class CreateMedicineCategoryDto
{
    public string CategoryCode { get; set; }
    public string CategoryName { get; set; }
    public string? Description { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}