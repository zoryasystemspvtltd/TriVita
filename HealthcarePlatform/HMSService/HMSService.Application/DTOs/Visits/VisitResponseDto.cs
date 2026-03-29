namespace HMSService.Application.DTOs.Visits;

public sealed class VisitResponseDto
{
    public long Id { get; set; }

    public long TenantId { get; set; }

    public long FacilityId { get; set; }

    public string VisitNo { get; set; } = null!;

    public long? AppointmentId { get; set; }

    public long PatientId { get; set; }

    public long DoctorId { get; set; }

    public long DepartmentId { get; set; }

    public long VisitTypeId { get; set; }

    public DateTime VisitStartOn { get; set; }

    public DateTime? VisitEndOn { get; set; }

    public string? ChiefComplaint { get; set; }

    public long CurrentStatusReferenceValueId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime ModifiedOn { get; set; }
}
