namespace LMSService.Application.DTOs.Entities;

public sealed class TechnicianAssignmentResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public long WorkQueueId { get; set; }
    public long TechnicianDoctorId { get; set; }
    public long AssignmentStatusReferenceValueId { get; set; }
    public DateTime AssignedOn { get; set; }
    public DateTime? ReleasedOn { get; set; }
    public string? Notes { get; set; }
}