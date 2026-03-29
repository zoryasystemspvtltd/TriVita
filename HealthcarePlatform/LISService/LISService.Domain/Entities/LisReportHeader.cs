using Healthcare.Common.Entities;

namespace LISService.Domain.Entities;

public sealed class LisReportHeader : BaseEntity
{
    public long LabOrderId { get; set; }
    public string ReportNo { get; set; } = null!;
    public long? ReportTypeReferenceValueId { get; set; }
    public long ReportStatusReferenceValueId { get; set; }
    public DateTime? PreparedOn { get; set; }
    public DateTime? ReviewedOn { get; set; }
    public DateTime? IssuedOn { get; set; }
    public long? PreparedByDoctorId { get; set; }
    public long? ReviewedByDoctorId { get; set; }
    public long? IssuedByDoctorId { get; set; }
}