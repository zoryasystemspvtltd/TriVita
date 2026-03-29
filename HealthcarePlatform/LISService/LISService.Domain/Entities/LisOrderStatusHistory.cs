using Healthcare.Common.Entities;

namespace LISService.Domain.Entities;

public sealed class LisOrderStatusHistory : BaseEntity
{
    public long LabOrderId { get; set; }
    public long StatusReferenceValueId { get; set; }
    public DateTime StatusOn { get; set; }
    public string? StatusNote { get; set; }
    public long? ChangedByDoctorId { get; set; }
}