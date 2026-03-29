using Healthcare.Common.Entities;

namespace PharmacyService.Domain.Entities;

public sealed class PhrComposition : BaseEntity
{
    public string CompositionName { get; set; } = null!;
    public string? CompositionCode { get; set; }
    public string? Notes { get; set; }
}