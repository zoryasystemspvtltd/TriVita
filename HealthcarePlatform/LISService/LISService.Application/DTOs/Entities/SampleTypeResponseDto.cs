namespace LISService.Application.DTOs.Entities;

public sealed class SampleTypeResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public string SampleTypeCode { get; set; }
    public string SampleTypeName { get; set; }
    public string? Description { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}