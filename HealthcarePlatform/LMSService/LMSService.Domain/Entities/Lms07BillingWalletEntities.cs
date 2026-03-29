using Healthcare.Common.Entities;

namespace LMSService.Domain.Entities;

public sealed class LmsLabInvoiceHeader : BaseEntity
{
    public string InvoiceNo { get; set; } = null!;
    public long LabOrderId { get; set; }
    public long PatientId { get; set; }
    public long? VisitId { get; set; }
    public long InvoiceStatusReferenceValueId { get; set; }
    public DateTime InvoiceDate { get; set; }
    public decimal? SubTotal { get; set; }
    public decimal? TaxTotal { get; set; }
    public decimal? DiscountTotal { get; set; }
    public decimal? GrandTotal { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal? BalanceDue { get; set; }
    public string? CurrencyCode { get; set; }
}

public sealed class LmsLabInvoiceLine : BaseEntity
{
    public long LabInvoiceHeaderId { get; set; }
    public int LineNum { get; set; }
    public long LineTypeReferenceValueId { get; set; }
    public long? LabOrderItemId { get; set; }
    public long? TestMasterId { get; set; }
    public long? TestPackageId { get; set; }
    public string? Description { get; set; }
    public decimal Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? LineSubTotal { get; set; }
    public decimal? TaxAmount { get; set; }
    public decimal? LineTotal { get; set; }
}

public sealed class LmsLabPaymentTransaction : BaseEntity
{
    public long LabInvoiceHeaderId { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionOn { get; set; }
    public long TransactionStatusReferenceValueId { get; set; }
    public long? PaymentModeId { get; set; }
    public long? GatewayProviderReferenceValueId { get; set; }
    public string? ExternalTransactionId { get; set; }
    public string? ReferenceNo { get; set; }
    public string? Notes { get; set; }
}

public sealed class LmsPatientWalletAccount : BaseEntity
{
    public long PatientId { get; set; }
    public string? CurrencyCode { get; set; }
    public decimal CurrentBalance { get; set; }
}

public sealed class LmsPatientWalletTransaction : BaseEntity
{
    public long PatientWalletAccountId { get; set; }
    public decimal AmountDelta { get; set; }
    public long WalletTxnTypeReferenceValueId { get; set; }
    public DateTime TransactionOn { get; set; }
    public long? LabInvoiceHeaderId { get; set; }
    public long? LabPaymentTransactionId { get; set; }
    public string? ReferenceNo { get; set; }
    public string? Notes { get; set; }
}
