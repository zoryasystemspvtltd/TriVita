namespace LISService.Application.DTOs.Entities;

public sealed class UpdateResultHistoryDto
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