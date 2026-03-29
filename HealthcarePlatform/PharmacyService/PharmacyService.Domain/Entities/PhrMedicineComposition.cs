using Healthcare.Common.Entities;

namespace PharmacyService.Domain.Entities;

public sealed class PhrMedicineComposition : BaseEntity
{
    public long MedicineId { get; set; }
    public long CompositionId { get; set; }
    public decimal? Amount { get; set; }
    public long? UnitId { get; set; }
}