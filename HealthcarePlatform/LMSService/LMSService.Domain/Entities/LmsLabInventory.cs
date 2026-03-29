using Healthcare.Common.Entities;

namespace LMSService.Domain.Entities;

public sealed class LmsLabInventory : BaseEntity
{
    public string InventoryItemCode { get; set; } = null!;
    public string InventoryItemName { get; set; } = null!;
    public long? UnitId { get; set; }
    public decimal CurrentQty { get; set; }
}