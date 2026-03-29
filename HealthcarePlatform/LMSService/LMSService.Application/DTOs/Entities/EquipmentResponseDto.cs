namespace LMSService.Application.DTOs.Entities;

public sealed class EquipmentResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public string EquipmentCode { get; set; }
    public string EquipmentName { get; set; }
    public long? EquipmentTypeReferenceValueId { get; set; }
    public string? SerialNumber { get; set; }
    public string? EquipmentNotes { get; set; }
}