namespace LISService.Application.DTOs.Entities;

public sealed class OrderStatusHistoryResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public long LabOrderId { get; set; }
    public long StatusReferenceValueId { get; set; }
    public DateTime StatusOn { get; set; }
    public string? StatusNote { get; set; }
    public long? ChangedByDoctorId { get; set; }
}