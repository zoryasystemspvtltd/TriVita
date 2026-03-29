using Healthcare.Common.Entities;

namespace LISService.Domain.Entities;

public sealed class LisResultHistory : BaseEntity
{
    public long LabResultsId { get; set; }
    public decimal? SnapshotResultNumeric { get; set; }
    public string? SnapshotResultText { get; set; }
    public bool SnapshotIsAbnormal { get; set; }
    public long? SnapshotAbnormalFlagReferenceValueId { get; set; }
    public long SnapshotResultStatusReferenceValueId { get; set; }
    public long? ChangedByDoctorId { get; set; }
    public string? ChangeReason { get; set; }
}