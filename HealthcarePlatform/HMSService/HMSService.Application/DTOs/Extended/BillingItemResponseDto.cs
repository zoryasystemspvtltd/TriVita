namespace HMSService.Application.DTOs.Extended;

public sealed class BillingItemResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public long BillingHeaderId { get; set; }
    public int LineNum { get; set; }
    public long ServiceTypeReferenceValueId { get; set; }
    public string? Description { get; set; }
    public decimal Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? LineTotal { get; set; }
    public long? LabOrderId { get; set; }
    public long? PrescriptionId { get; set; }
    public long? PharmacySalesId { get; set; }
    public string? ExternalReference { get; set; }
}