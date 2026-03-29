using Healthcare.Common.Entities;

namespace HMSService.Domain.Entities;

public sealed class HmsBillingHeader : BaseEntity
{
    public string BillNo { get; set; } = null!;
    public long VisitId { get; set; }
    public long PatientId { get; set; }
    public DateTime BillDate { get; set; }
    public long BillingStatusReferenceValueId { get; set; }
    public decimal? SubTotal { get; set; }
    public decimal? TaxTotal { get; set; }
    public decimal? DiscountTotal { get; set; }
    public decimal? GrandTotal { get; set; }
    public string? CurrencyCode { get; set; }
    public string? Notes { get; set; }
}