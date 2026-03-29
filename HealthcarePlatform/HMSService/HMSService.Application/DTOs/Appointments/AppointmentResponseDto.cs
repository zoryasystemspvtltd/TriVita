namespace HMSService.Application.DTOs.Appointments;

/// <summary>Appointment returned by the API (never expose domain entities).</summary>
public sealed class AppointmentResponseDto
{
    /// <summary>Surrogate key.</summary>
    public long Id { get; set; }

    public long TenantId { get; set; }

    public long FacilityId { get; set; }

    public string AppointmentNo { get; set; } = null!;

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

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime ModifiedOn { get; set; }
}
