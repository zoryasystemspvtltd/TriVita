using Healthcare.Common.Entities;

namespace PharmacyService.Domain.Entities;

public sealed class PhrPrescriptionMapping : BaseEntity
{
    public long PrescriptionId { get; set; }
    public long PharmacySalesId { get; set; }
    public long? PrescriptionItemId { get; set; }
    public long? PharmacySalesItemId { get; set; }
    public decimal? MappedQty { get; set; }
    public string? MappingNotes { get; set; }
}