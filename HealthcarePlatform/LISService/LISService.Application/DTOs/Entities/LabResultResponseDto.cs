namespace LISService.Application.DTOs.Entities;

public sealed class LabResultResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public long LabOrderItemId { get; set; }
    public long TestParameterId { get; set; }
    public decimal? ResultNumeric { get; set; }
    public string? ResultText { get; set; }
    public long? ResultUnitId { get; set; }
    public bool IsAbnormal { get; set; }
    public long? AbnormalFlagReferenceValueId { get; set; }
    public string? Notes { get; set; }
    public DateTime? MeasuredOn { get; set; }
    public long ResultStatusReferenceValueId { get; set; }
    public long? EnteredByDoctorId { get; set; }
}