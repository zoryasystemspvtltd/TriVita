using Healthcare.Common.Entities;

namespace PharmacyService.Domain.Entities;

public sealed class PhrReferenceDataDefinition : BaseEntity
{
    public string DefinitionCode { get; set; } = null!;
    public string DefinitionName { get; set; } = null!;
    public string? Description { get; set; }
}
