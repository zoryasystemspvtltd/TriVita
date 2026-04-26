using Healthcare.Common.Entities;

namespace PharmacyService.Domain.Entities;

/// <summary>Measurement / dispensing unit (dbo.Unit). UnitSymbol maps to UnitType column.</summary>
public sealed class PhrUnit : BaseEntity
{
    public string UnitCode { get; set; } = null!;
    public string UnitName { get; set; } = null!;
    public string? UnitSymbol { get; set; }
    public string? Description { get; set; }
}
