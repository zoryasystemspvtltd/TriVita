namespace HMSService.Application.DTOs.Extended;

public sealed class CreateBillingHeaderDto
{
    /// <summary>Optional; generated server-side when empty.</summary>
    public string? BillNo { get; set; }
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