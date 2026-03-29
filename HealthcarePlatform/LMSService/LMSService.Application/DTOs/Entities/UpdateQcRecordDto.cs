namespace LMSService.Application.DTOs.Entities;

public sealed class UpdateQcRecordDto
{
    public long? QCTypeReferenceValueId { get; set; }
    public long QCStatusReferenceValueId { get; set; }
    public DateTime? ScheduledOn { get; set; }
    public DateTime? PerformedOn { get; set; }
    public long? PerformedByDoctorId { get; set; }
    public string? LotNo { get; set; }
    public string? Notes { get; set; }
}