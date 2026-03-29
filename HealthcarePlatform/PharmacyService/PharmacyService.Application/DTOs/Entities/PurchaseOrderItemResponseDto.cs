namespace PharmacyService.Application.DTOs.Entities;

public sealed class PurchaseOrderItemResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public long PurchaseOrderId { get; set; }
    public int LineNum { get; set; }
    public long MedicineId { get; set; }
    public decimal QuantityOrdered { get; set; }
    public long? UnitId { get; set; }
    public decimal? PurchaseRate { get; set; }
    public string? Notes { get; set; }
}