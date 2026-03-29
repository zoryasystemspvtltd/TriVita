namespace PharmacyService.Application.DTOs.Entities;

public sealed class CreatePrescriptionMappingDto
{
    public long PrescriptionId { get; set; }
    public long PharmacySalesId { get; set; }
    public long? PrescriptionItemId { get; set; }
    public long? PharmacySalesItemId { get; set; }
    public decimal? MappedQty { get; set; }
    public string? MappingNotes { get; set; }
}