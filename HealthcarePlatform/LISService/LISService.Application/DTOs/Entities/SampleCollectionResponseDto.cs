namespace LISService.Application.DTOs.Entities;

public sealed class SampleCollectionResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public long LabOrderItemId { get; set; }
    public long SampleTypeId { get; set; }
    public DateTime CollectedOn { get; set; }
    public long? CollectedByDoctorId { get; set; }
    public long? CollectionDepartmentId { get; set; }
    public decimal? CollectedQuantity { get; set; }
    public long? CollectedQuantityUnitId { get; set; }
    public string? CollectionNotes { get; set; }
}