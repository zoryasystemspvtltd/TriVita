namespace LISService.Application.DTOs.Entities;

public sealed class UpdateTestCategoryDto
{
    public string CategoryCode { get; set; }
    public string CategoryName { get; set; }
    public long? ParentCategoryId { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}