namespace LISService.Application.DTOs.Entities;

public sealed class UpdateTestReferenceRangeDto
{
    public long TestParameterId { get; set; }
    public long? SexReferenceValueId { get; set; }
    public int? AgeFromYears { get; set; }
    public int? AgeToYears { get; set; }
    public long? ReferenceRangeTypeReferenceValueId { get; set; }
    public decimal? MinValue { get; set; }
    public decimal? MaxValue { get; set; }
    public long? RangeUnitId { get; set; }
    public string? RangeNotes { get; set; }
    public DateTime? EffectiveFromDate { get; set; }
    public DateTime? EffectiveToDate { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}