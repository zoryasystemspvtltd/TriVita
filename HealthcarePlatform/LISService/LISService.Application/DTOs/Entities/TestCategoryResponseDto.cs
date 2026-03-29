namespace LISService.Application.DTOs.Entities;

public sealed class TestCategoryResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public string CategoryCode { get; set; }
    public string CategoryName { get; set; }
    public long? ParentCategoryId { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}