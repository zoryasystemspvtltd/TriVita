namespace LMSService.Application.DTOs.Entities;

public sealed class UpdateLabInventoryTransactionDto
{
    public long LabInventoryId { get; set; }
    public long? TransactionTypeReferenceValueId { get; set; }
    public decimal QuantityDelta { get; set; }
    public DateTime TransactionOn { get; set; }
    public long? PerformedByDoctorId { get; set; }
    public string? Notes { get; set; }
}