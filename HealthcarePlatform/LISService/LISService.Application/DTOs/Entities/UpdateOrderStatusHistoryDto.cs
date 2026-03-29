namespace LISService.Application.DTOs.Entities;

public sealed class UpdateOrderStatusHistoryDto
{
    public long LabOrderId { get; set; }
    public long StatusReferenceValueId { get; set; }
    public DateTime StatusOn { get; set; }
    public string? StatusNote { get; set; }
    public long? ChangedByDoctorId { get; set; }
}