namespace PharmacyService.Application.DTOs.Entities;

public sealed class BatchStockResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public long MedicineBatchId { get; set; }
    public decimal CurrentQty { get; set; }
    public decimal ReservedQty { get; set; }
    public decimal AvailableQty { get; set; }
    public decimal? ReorderLevelQty { get; set; }
    public DateTime? LastUpdatedOn { get; set; }
}