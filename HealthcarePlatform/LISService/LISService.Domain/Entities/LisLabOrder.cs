using Healthcare.Common.Entities;

namespace LISService.Domain.Entities;

public sealed class LisLabOrder : BaseEntity
{
    public string LabOrderNo { get; set; } = null!;
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