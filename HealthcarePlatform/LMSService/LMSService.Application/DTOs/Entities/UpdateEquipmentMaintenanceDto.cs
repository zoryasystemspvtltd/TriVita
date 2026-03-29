namespace LMSService.Application.DTOs.Entities;

public sealed class UpdateEquipmentMaintenanceDto
{
    public long EquipmentId { get; set; }
    public long? MaintenanceTypeReferenceValueId { get; set; }
    public long? MaintenanceStatusReferenceValueId { get; set; }
    public DateTime? ScheduledOn { get; set; }
    public DateTime? PerformedOn { get; set; }
    public long? PerformedByDoctorId { get; set; }
    public string? MaintenanceNotes { get; set; }
    public DateTime? NextDueOn { get; set; }
}