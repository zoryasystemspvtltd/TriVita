using Healthcare.Common.Entities;

namespace HMSService.Domain.Entities;

public sealed class HmsAppointmentStatusHistory : BaseEntity
{
    public long AppointmentId { get; set; }
    public long StatusValueId { get; set; }
    public DateTime StatusOn { get; set; }
    public string? StatusNote { get; set; }
    public long? ChangedByDoctorId { get; set; }
}