namespace LISService.Application.DTOs.Entities;

public sealed class UpdateLabOrderDto
{
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