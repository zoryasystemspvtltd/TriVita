namespace PharmacyService.Application.DTOs.Entities;

public sealed class UnitResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public string UnitCode { get; set; } = null!;
    public string UnitName { get; set; } = null!;
    public string UnitSymbol { get; set; } = null!;
    public string? Description { get; set; }
}
