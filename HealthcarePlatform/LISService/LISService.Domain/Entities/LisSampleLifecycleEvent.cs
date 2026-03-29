using Healthcare.Common.Entities;

namespace LISService.Domain.Entities;

public sealed class LisSampleLifecycleEvent : BaseEntity
{
    public long SampleCollectionId { get; set; }
    public long? LabOrderItemId { get; set; }
    public long EventTypeReferenceValueId { get; set; }
    public DateTime EventOn { get; set; }
    public DateTime? PlannedDueOn { get; set; }
    public bool TatBreached { get; set; }
    public long? LocationDepartmentId { get; set; }
    public string? EventNotes { get; set; }
}
