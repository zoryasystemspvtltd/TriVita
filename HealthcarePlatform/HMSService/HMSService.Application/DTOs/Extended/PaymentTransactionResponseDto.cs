namespace HMSService.Application.DTOs.Extended;

public sealed class PaymentTransactionResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public long BillingHeaderId { get; set; }
    public long PaymentModeId { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionOn { get; set; }
    public long TransactionStatusReferenceValueId { get; set; }
    public string? ReferenceNo { get; set; }
    public string? Notes { get; set; }
}