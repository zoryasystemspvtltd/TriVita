using Healthcare.Common.Entities;

namespace PharmacyService.Domain.Entities;

public sealed class PhrMedicineBatch : BaseEntity
{
    public long MedicineId { get; set; }
    public string BatchNo { get; set; } = null!;
    public DateTime? ExpiryDate { get; set; }
    public decimal? MRP { get; set; }
    public decimal? PurchaseRate { get; set; }
    public DateTime? ManufacturingDate { get; set; }
}