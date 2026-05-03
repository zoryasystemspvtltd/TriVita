using Healthcare.Common.Entities;
using PharmacyService.Domain.Enums;

namespace PharmacyService.Domain.Entities;

public sealed class PhrSalesBill : BaseEntity
{
    public string BillNo { get; set; } = null!;
    public long? CustomerId { get; set; }
    public long? PatientId { get; set; }
    public DateTime SalesDate { get; set; }
    public PharmacySalesBillStatus Status { get; set; } = PharmacySalesBillStatus.Draft;

    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal GstPercent { get; set; }
    public decimal GstAmount { get; set; }
    public decimal OtherTaxAmount { get; set; }
    public decimal NetAmount { get; set; }
    public string? Notes { get; set; }
}
