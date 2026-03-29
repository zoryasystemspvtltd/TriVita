using Healthcare.Common.Entities;

namespace HMSService.Domain.Entities;

public sealed class HmsAppointment : BaseEntity
{
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

    public HmsVisitType? VisitType { get; set; }
}
