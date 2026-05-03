using Healthcare.Common.Entities;
using PharmacyService.Domain.Enums;

namespace PharmacyService.Domain.Entities;

public sealed class PhrStockAdjustment : BaseEntity
{
    public string AdjustmentNo { get; set; } = null!;
    public DateTime AdjustmentOn { get; set; }
    public PharmacyStockAdjustmentStatus Status { get; set; } = PharmacyStockAdjustmentStatus.Draft;
    public long? AdjustmentTypeReferenceValueId { get; set; }
    public long? PerformedByDoctorId { get; set; }
    public string? ReasonNotes { get; set; }
}