namespace LISService.Application.DTOs.Entities;

public sealed class ReportHeaderResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public long LabOrderId { get; set; }
    public string ReportNo { get; set; }
    public long? ReportTypeReferenceValueId { get; set; }
    public long ReportStatusReferenceValueId { get; set; }
    public DateTime? PreparedOn { get; set; }
    public DateTime? ReviewedOn { get; set; }
    public DateTime? IssuedOn { get; set; }
    public long? PreparedByDoctorId { get; set; }
    public long? ReviewedByDoctorId { get; set; }
    public long? IssuedByDoctorId { get; set; }
}