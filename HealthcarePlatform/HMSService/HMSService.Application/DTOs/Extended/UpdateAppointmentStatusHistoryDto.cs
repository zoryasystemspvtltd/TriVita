namespace HMSService.Application.DTOs.Extended;

public sealed class UpdateAppointmentStatusHistoryDto
{
    public long AppointmentId { get; set; }
    public long StatusValueId { get; set; }
    public DateTime StatusOn { get; set; }
    public string? StatusNote { get; set; }
    public long? ChangedByDoctorId { get; set; }
}