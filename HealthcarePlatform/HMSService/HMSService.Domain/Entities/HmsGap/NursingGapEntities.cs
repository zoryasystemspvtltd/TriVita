using Healthcare.Common.Entities;

namespace HMSService.Domain.Entities;

public sealed class HmsEmarEntry : BaseEntity
{
    public long AdmissionId { get; set; }

    public string MedicationCode { get; set; } = null!;

    public DateTime ScheduledOn { get; set; }

    public DateTime? AdministeredOn { get; set; }

    public long AdministrationStatusReferenceValueId { get; set; }

    public long? NurseUserId { get; set; }

    public string? Notes { get; set; }
}

public sealed class HmsDoctorOrderAlert : BaseEntity
{
    public long? VisitId { get; set; }

    public long? AdmissionId { get; set; }

    public long DoctorId { get; set; }

    public long AlertTypeReferenceValueId { get; set; }

    public string Message { get; set; } = null!;

    public DateTime? AcknowledgedOn { get; set; }
}
