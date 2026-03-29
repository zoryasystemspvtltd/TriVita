namespace LMSService.Application.DTOs.Entities;

public sealed class UpdateLabInventoryDto
{
    public string InventoryItemCode { get; set; }
    public string InventoryItemName { get; set; }
    public long? UnitId { get; set; }
    public decimal CurrentQty { get; set; }
}