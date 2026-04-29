namespace PharmacyService.Application.DTOs.Entities;

public sealed class PurchaseOrderItemUpsertDto
{
    public long? Id { get; set; }
    public long MedicineId { get; set; }
    public decimal QuantityOrdered { get; set; }
    public decimal UnitPrice { get; set; }
    public string? Notes { get; set; }
}

