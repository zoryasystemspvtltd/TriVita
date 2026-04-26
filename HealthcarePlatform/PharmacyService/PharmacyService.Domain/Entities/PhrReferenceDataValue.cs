using Healthcare.Common.Entities;

namespace PharmacyService.Domain.Entities;

/// <summary>Dosage form and other reference values (dbo.ReferenceDataValue).</summary>
public sealed class PhrReferenceDataValue : BaseEntity
{
    public long ReferenceDataDefinitionId { get; set; }
    public string ValueCode { get; set; } = null!;
    public string ValueName { get; set; } = null!;
    public string? ValueText { get; set; }
    public int SortOrder { get; set; }
}
