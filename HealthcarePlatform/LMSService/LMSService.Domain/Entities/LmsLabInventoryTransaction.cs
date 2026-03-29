using Healthcare.Common.Entities;

namespace LMSService.Domain.Entities;

public sealed class LmsLabInventoryTransaction : BaseEntity
{
    public long LabInventoryId { get; set; }
    public long? TransactionTypeReferenceValueId { get; set; }
    public decimal QuantityDelta { get; set; }
    public DateTime TransactionOn { get; set; }
    public long? PerformedByDoctorId { get; set; }
    public string? Notes { get; set; }
}