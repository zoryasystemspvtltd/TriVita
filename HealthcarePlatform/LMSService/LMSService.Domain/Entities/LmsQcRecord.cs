using Healthcare.Common.Entities;

namespace LMSService.Domain.Entities;

public sealed class LmsQcRecord : BaseEntity
{
    public long? QCTypeReferenceValueId { get; set; }
    public long QCStatusReferenceValueId { get; set; }
    public DateTime? ScheduledOn { get; set; }
    public DateTime? PerformedOn { get; set; }
    public long? PerformedByDoctorId { get; set; }
    public string? LotNo { get; set; }
    public string? Notes { get; set; }
}