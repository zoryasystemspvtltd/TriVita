namespace PharmacyService.Application.DTOs.Entities;

public sealed class SalesBillItemResponseDto
{
    public long Id { get; set; }
    public long SalesBillId { get; set; }
    public int LineNum { get; set; }
    public long MedicineId { get; set; }
    public string MedicineName { get; set; } = "";
    public long? MedicineBatchId { get; set; }
    public string? BatchNo { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}
