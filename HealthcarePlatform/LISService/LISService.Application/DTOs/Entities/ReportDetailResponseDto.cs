namespace LISService.Application.DTOs.Entities;

public sealed class ReportDetailResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public long ReportHeaderId { get; set; }
    public int LineNum { get; set; }
    public long? TestMasterId { get; set; }
    public long? TestParameterId { get; set; }
    public string? ResultDisplayText { get; set; }
    public string? ReferenceRangeDisplayText { get; set; }
    public string? LineNotes { get; set; }
}