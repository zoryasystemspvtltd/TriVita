namespace HMSService.Application.DTOs.Visits;

public sealed class CreateVisitDto
{
    public long? AppointmentId { get; set; }

    public long PatientId { get; set; }

    public long DoctorId { get; set; }

    public long DepartmentId { get; set; }

    public long VisitTypeId { get; set; }

    public DateTime VisitStartOn { get; set; }

    public DateTime? VisitEndOn { get; set; }

    public string? ChiefComplaint { get; set; }

    public long CurrentStatusReferenceValueId { get; set; }
}
