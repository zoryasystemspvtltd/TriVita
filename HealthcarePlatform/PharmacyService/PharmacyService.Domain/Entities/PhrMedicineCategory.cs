using Healthcare.Common.Entities;

namespace PharmacyService.Domain.Entities;

public sealed class PhrMedicineCategory : BaseEntity
{
    public string CategoryCode { get; set; } = null!;
    public string CategoryName { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}