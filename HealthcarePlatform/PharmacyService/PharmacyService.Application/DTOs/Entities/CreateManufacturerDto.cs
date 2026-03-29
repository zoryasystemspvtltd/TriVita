namespace PharmacyService.Application.DTOs.Entities;

public sealed class CreateManufacturerDto
{
    public string? ManufacturerCode { get; set; }
    public string ManufacturerName { get; set; }
    public string? CountryCode { get; set; }
}