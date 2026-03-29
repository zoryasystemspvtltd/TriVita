namespace LMSService.Application.DTOs.Entities;

public sealed class LabInventoryResponseDto
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long? FacilityId { get; set; }
    public string InventoryItemCode { get; set; }
    public string InventoryItemName { get; set; }
    public long? UnitId { get; set; }
    public decimal CurrentQty { get; set; }
}