namespace LISService.Application.DTOs.Entities;

public sealed class TestMasterResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public long CategoryId { get; set; }
    public long? SampleTypeId { get; set; }
    public string TestCode { get; set; }
    public string TestName { get; set; }
    public string? TestDescription { get; set; }
    public long? DefaultUnitId { get; set; }
    public bool IsQuantitative { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}