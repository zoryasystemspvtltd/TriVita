namespace PharmacyService.Application.DTOs.Entities;

public sealed class UpdateManufacturerDto
{
    public string? ManufacturerCode { get; set; }
    public string ManufacturerName { get; set; }
    public string? CountryCode { get; set; }
}