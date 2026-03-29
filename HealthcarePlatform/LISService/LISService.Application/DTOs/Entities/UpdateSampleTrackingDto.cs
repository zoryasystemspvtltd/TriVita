namespace LISService.Application.DTOs.Entities;

public sealed class UpdateSampleTrackingDto
{
    public long SampleCollectionId { get; set; }
    public string TrackingNo { get; set; }
    public long? TrackingEventTypeReferenceValueId { get; set; }
    public long? TrackingStatusReferenceValueId { get; set; }
    public long? LocationDepartmentId { get; set; }
    public DateTime TrackedOn { get; set; }
    public long? ScannedByDoctorId { get; set; }
    public string? TrackingNotes { get; set; }
}