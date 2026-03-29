namespace LMSService.Application.DTOs.Entities;

public sealed class EquipmentCalibrationResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public long EquipmentId { get; set; }
    public DateTime CalibratedOn { get; set; }
    public long? CalibratorDoctorId { get; set; }
    public decimal? ResultNumeric { get; set; }
    public string? ResultText { get; set; }
    public DateTime? ValidUntil { get; set; }
    public bool IsWithinTolerance { get; set; }
    public string? Comments { get; set; }
}