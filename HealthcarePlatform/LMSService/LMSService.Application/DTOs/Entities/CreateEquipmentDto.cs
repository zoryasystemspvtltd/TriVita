namespace LMSService.Application.DTOs.Entities;

public sealed class CreateEquipmentDto
{
    public string EquipmentCode { get; set; }
    public string EquipmentName { get; set; }
    public long? EquipmentTypeReferenceValueId { get; set; }
    public string? SerialNumber { get; set; }
    public string? EquipmentNotes { get; set; }
}