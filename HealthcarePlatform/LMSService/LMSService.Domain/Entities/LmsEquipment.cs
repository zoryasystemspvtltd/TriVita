using Healthcare.Common.Entities;

namespace LMSService.Domain.Entities;

public sealed class LmsEquipment : BaseEntity
{
    public string EquipmentCode { get; set; } = null!;
    public string EquipmentName { get; set; } = null!;
    public long? EquipmentTypeReferenceValueId { get; set; }
    public string? SerialNumber { get; set; }
    public string? EquipmentNotes { get; set; }
}