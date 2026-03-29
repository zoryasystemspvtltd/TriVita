namespace PharmacyService.Application.DTOs.Entities;

public sealed class ManufacturerResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public string? ManufacturerCode { get; set; }
    public string ManufacturerName { get; set; }
    public string? CountryCode { get; set; }
}