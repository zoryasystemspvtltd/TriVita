namespace HMSService.Application.DTOs.Extended;

public sealed class AppointmentStatusHistoryResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public long AppointmentId { get; set; }
    public long StatusValueId { get; set; }
    public DateTime StatusOn { get; set; }
    public string? StatusNote { get; set; }
    public long? ChangedByDoctorId { get; set; }
}