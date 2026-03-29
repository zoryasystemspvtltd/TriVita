namespace LISService.Application.DTOs.Entities;

public sealed class SampleTrackingResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public long SampleCollectionId { get; set; }
    public string TrackingNo { get; set; }
    public long? TrackingEventTypeReferenceValueId { get; set; }
    public long? TrackingStatusReferenceValueId { get; set; }
    public long? LocationDepartmentId { get; set; }
    public DateTime TrackedOn { get; set; }
    public long? ScannedByDoctorId { get; set; }
    public string? TrackingNotes { get; set; }
}