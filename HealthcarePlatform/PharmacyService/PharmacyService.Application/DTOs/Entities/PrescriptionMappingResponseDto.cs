namespace PharmacyService.Application.DTOs.Entities;

public sealed class PrescriptionMappingResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public long PrescriptionId { get; set; }
    public long PharmacySalesId { get; set; }
    public long? PrescriptionItemId { get; set; }
    public long? PharmacySalesItemId { get; set; }
    public decimal? MappedQty { get; set; }
    public string? MappingNotes { get; set; }
}