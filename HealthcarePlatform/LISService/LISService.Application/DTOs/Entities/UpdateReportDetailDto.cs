namespace LISService.Application.DTOs.Entities;

public sealed class UpdateReportDetailDto
{
    public long ReportHeaderId { get; set; }
    public int LineNum { get; set; }
    public long? TestMasterId { get; set; }
    public long? TestParameterId { get; set; }
    public string? ResultDisplayText { get; set; }
    public string? ReferenceRangeDisplayText { get; set; }
    public string? LineNotes { get; set; }
}