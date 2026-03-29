using Healthcare.Common.Entities;

namespace LISService.Domain.Entities;

public sealed class LisReportLockState : BaseEntity
{
    public long ReportHeaderId { get; set; }
    public bool IsLocked { get; set; }
    public DateTime? LockedOn { get; set; }
    public long? LockedByUserId { get; set; }
    public long? LockReasonReferenceValueId { get; set; }
}
