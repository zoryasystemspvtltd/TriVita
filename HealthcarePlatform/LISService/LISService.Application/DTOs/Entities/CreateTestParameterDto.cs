namespace LISService.Application.DTOs.Entities;

public sealed class CreateTestParameterDto
{
    public long TestMasterId { get; set; }
    public string ParameterCode { get; set; }
    public string ParameterName { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsNumeric { get; set; }
    public long? UnitId { get; set; }
    public string? ParameterNotes { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}