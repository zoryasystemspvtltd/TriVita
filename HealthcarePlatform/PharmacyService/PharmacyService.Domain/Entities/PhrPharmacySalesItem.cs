using Healthcare.Common.Entities;

namespace PharmacyService.Domain.Entities;

public sealed class PhrPharmacySalesItem : BaseEntity
{
    public long PharmacySalesId { get; set; }
    public int LineNum { get; set; }
    public long MedicineId { get; set; }
    public long MedicineBatchId { get; set; }
    public decimal QuantitySold { get; set; }
    public long? UnitId { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? LineTotal { get; set; }
    public DateTime? DispensedOn { get; set; }
    public string? Notes { get; set; }
}