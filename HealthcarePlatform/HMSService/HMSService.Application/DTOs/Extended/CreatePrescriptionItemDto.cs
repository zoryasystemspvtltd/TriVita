namespace HMSService.Application.DTOs.Extended;

public sealed class CreatePrescriptionItemDto
{
    public long PrescriptionId { get; set; }
    public int LineNum { get; set; }
    public long MedicineId { get; set; }
    public long? UnitId { get; set; }
    public decimal? Quantity { get; set; }
    public string? DosageText { get; set; }
    public string? FrequencyText { get; set; }
    public int? DurationDays { get; set; }
    public long? RouteReferenceValueId { get; set; }
    public bool IsPRN { get; set; }
    public string? ItemNotes { get; set; }
}