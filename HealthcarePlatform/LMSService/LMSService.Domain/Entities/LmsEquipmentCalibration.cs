using Healthcare.Common.Entities;

namespace LMSService.Domain.Entities;

public sealed class LmsEquipmentCalibration : BaseEntity
{
    public long EquipmentId { get; set; }
    public DateTime CalibratedOn { get; set; }
    public long? CalibratorDoctorId { get; set; }
    public decimal? ResultNumeric { get; set; }
    public string? ResultText { get; set; }
    public DateTime? ValidUntil { get; set; }
    public bool IsWithinTolerance { get; set; }
    public string? Comments { get; set; }
}