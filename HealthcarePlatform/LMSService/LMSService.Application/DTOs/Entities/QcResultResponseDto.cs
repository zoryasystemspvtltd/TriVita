namespace LMSService.Application.DTOs.Entities;

public sealed class QcResultResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public long QCRecordId { get; set; }
    public long? TestParameterId { get; set; }
    public decimal? ResultNumeric { get; set; }
    public string? ResultText { get; set; }
    public long? ResultUnitId { get; set; }
    public bool IsPass { get; set; }
    public string? Notes { get; set; }
}