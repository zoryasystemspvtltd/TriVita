using Healthcare.Common.Entities;

namespace PharmacyService.Domain.Entities;

public sealed class PhrMedicine : BaseEntity
{
    public string MedicineCode { get; set; } = null!;
    public string MedicineName { get; set; } = null!;
    public long CategoryId { get; set; }
    public long? ManufacturerId { get; set; }
    public string? Strength { get; set; }
    public long? DefaultUnitId { get; set; }
    public long? FormReferenceValueId { get; set; }
    /// <summary>Optional primary ingredient (Composition master) for display and formulary.</summary>
    public long? PrimaryCompositionId { get; set; }
    public string? Notes { get; set; }
}