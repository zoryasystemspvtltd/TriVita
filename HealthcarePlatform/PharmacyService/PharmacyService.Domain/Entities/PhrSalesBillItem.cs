using Healthcare.Common.Entities;

namespace PharmacyService.Domain.Entities;

public sealed class PhrSalesBillItem : BaseEntity
{
    public long SalesBillId { get; set; }
    public int LineNum { get; set; }
    public long MedicineId { get; set; }
    public long? MedicineBatchId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}
