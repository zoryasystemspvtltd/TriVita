using Healthcare.Common.Entities;

namespace LISService.Domain.Entities;

public sealed class LisLabResult : BaseEntity
{
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