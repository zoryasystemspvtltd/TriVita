namespace PharmacyService.Application.DTOs.Entities;

public sealed class CreateUnitDto
{
    public string UnitCode { get; set; } = null!;
    public string UnitName { get; set; } = null!;
    public string UnitSymbol { get; set; } = null!;
    public string? Description { get; set; }
}
