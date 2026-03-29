namespace LMSService.Application.DTOs.Entities;

public sealed class WorkQueueResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public long LabOrderItemId { get; set; }
    public long StageId { get; set; }
    public long? PriorityReferenceValueId { get; set; }
    public long QueueStatusReferenceValueId { get; set; }
    public DateTime QueuedOn { get; set; }
    public DateTime? ClaimedOn { get; set; }
    public DateTime? CompletedOn { get; set; }
    public long? AssignedByDoctorId { get; set; }
    public long? AssignedTechnicianDoctorId { get; set; }
    public string? QueueNotes { get; set; }
}