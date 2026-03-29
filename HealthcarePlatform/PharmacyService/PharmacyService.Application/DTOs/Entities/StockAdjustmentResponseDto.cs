namespace PharmacyService.Application.DTOs.Entities;

public sealed class StockAdjustmentResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public string AdjustmentNo { get; set; }
    public DateTime AdjustmentOn { get; set; }
    public long? AdjustmentTypeReferenceValueId { get; set; }
    public long? PerformedByDoctorId { get; set; }
    public string? ReasonNotes { get; set; }
}