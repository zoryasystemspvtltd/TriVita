using Healthcare.Common.Entities;

namespace PharmacyService.Domain.Entities;

public sealed class PhrManufacturer : BaseEntity
{
    public string? ManufacturerCode { get; set; }
    public string ManufacturerName { get; set; } = null!;
    public string? CountryCode { get; set; }
}