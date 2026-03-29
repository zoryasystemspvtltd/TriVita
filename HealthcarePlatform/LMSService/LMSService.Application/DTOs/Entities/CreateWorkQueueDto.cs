namespace LMSService.Application.DTOs.Entities;

public sealed class CreateWorkQueueDto
{
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