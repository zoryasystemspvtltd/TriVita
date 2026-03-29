namespace PharmacyService.Application.DTOs.Entities;

public sealed class UpdateStockAdjustmentDto
{
    public string AdjustmentNo { get; set; }
    public DateTime AdjustmentOn { get; set; }
    public long? AdjustmentTypeReferenceValueId { get; set; }
    public long? PerformedByDoctorId { get; set; }
    public string? ReasonNotes { get; set; }
}