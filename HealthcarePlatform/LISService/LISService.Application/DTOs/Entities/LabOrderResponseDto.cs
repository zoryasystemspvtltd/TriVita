namespace LISService.Application.DTOs.Entities;

public sealed class LabOrderResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public string LabOrderNo { get; set; }
    public long PatientId { get; set; }
    public long? VisitId { get; set; }
    public long? OrderingDoctorId { get; set; }
    public long? DepartmentId { get; set; }
    public DateTime OrderedOn { get; set; }
    public long OrderStatusReferenceValueId { get; set; }
    public long? PriorityReferenceValueId { get; set; }
    public string? ClinicalNotes { get; set; }
    public DateTime? RequestedCollectionOn { get; set; }
}