namespace LISService.Application.DTOs.Entities;

public sealed class CreateSampleTypeDto
{
    public string SampleTypeCode { get; set; }
    public string SampleTypeName { get; set; }
    public string? Description { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}