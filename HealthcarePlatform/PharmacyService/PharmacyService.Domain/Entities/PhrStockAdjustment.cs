using Healthcare.Common.Entities;

namespace PharmacyService.Domain.Entities;

public sealed class PhrStockAdjustment : BaseEntity
{
    public string AdjustmentNo { get; set; } = null!;
    public DateTime AdjustmentOn { get; set; }
    public long? AdjustmentTypeReferenceValueId { get; set; }
    public long? PerformedByDoctorId { get; set; }
    public string? ReasonNotes { get; set; }
}