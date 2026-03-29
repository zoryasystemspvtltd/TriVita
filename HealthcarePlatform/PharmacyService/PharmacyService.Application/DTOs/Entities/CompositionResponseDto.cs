namespace PharmacyService.Application.DTOs.Entities;

public sealed class CompositionResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public string CompositionName { get; set; }
    public string? CompositionCode { get; set; }
    public string? Notes { get; set; }
}