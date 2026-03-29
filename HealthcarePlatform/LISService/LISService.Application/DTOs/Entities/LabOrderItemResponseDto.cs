namespace LISService.Application.DTOs.Entities;

public sealed class LabOrderItemResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public long LabOrderId { get; set; }
    public long TestMasterId { get; set; }
    public long? SampleTypeId { get; set; }
    public int LineNum { get; set; }
    public DateTime RequestedOn { get; set; }
    public long OrderItemStatusReferenceValueId { get; set; }
    public decimal? SpecimenVolume { get; set; }
    public long? SpecimenVolumeUnitId { get; set; }
    public string? Notes { get; set; }
}