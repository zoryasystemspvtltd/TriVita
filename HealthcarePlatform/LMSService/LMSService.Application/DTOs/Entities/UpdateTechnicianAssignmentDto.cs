namespace LMSService.Application.DTOs.Entities;

public sealed class UpdateTechnicianAssignmentDto
{
    public long WorkQueueId { get; set; }
    public long TechnicianDoctorId { get; set; }
    public long AssignmentStatusReferenceValueId { get; set; }
    public DateTime AssignedOn { get; set; }
    public DateTime? ReleasedOn { get; set; }
    public string? Notes { get; set; }
}