namespace HMSService.Application.Integration;

/// <summary>Subset of LMS lab invoice for HMS billing / payment tracking views.</summary>
public sealed class LabInvoiceSummaryDto
{
    public long Id { get; set; }
    public long FacilityId { get; set; }
    public string InvoiceNo { get; set; } = null!;
    public long LabOrderId { get; set; }
    public long PatientId { get; set; }
    public long? VisitId { get; set; }
    public DateTime InvoiceDate { get; set; }
    public decimal? GrandTotal { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal? BalanceDue { get; set; }
}
