namespace HMSService.Application.DTOs.Appointments;

/// <summary>Payload to create an OPD appointment.</summary>
public sealed class CreateAppointmentDto
{
    /// <summary>Shared-domain patient identifier.</summary>
    public long PatientId { get; set; }

    public long DoctorId { get; set; }

    public long DepartmentId { get; set; }

    public long? VisitTypeId { get; set; }

    public long AppointmentStatusValueId { get; set; }

    public DateTime ScheduledStartOn { get; set; }

    public DateTime? ScheduledEndOn { get; set; }

    public long? PriorityReferenceValueId { get; set; }

    public string? Reason { get; set; }

    public DateTime? EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }
}
