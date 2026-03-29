namespace LMSService.Application.DTOs.Entities;

public sealed class UpdateQcResultDto
{
    public long QCRecordId { get; set; }
    public long? TestParameterId { get; set; }
    public decimal? ResultNumeric { get; set; }
    public string? ResultText { get; set; }
    public long? ResultUnitId { get; set; }
    public bool IsPass { get; set; }
    public string? Notes { get; set; }
}