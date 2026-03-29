namespace LMSService.Application.DTOs.Entities;

public sealed class CreateLabInventoryDto
{
    public string InventoryItemCode { get; set; }
    public string InventoryItemName { get; set; }
    public long? UnitId { get; set; }
    public decimal CurrentQty { get; set; }
}