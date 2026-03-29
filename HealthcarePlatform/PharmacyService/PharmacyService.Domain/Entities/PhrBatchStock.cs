using Healthcare.Common.Entities;

namespace PharmacyService.Domain.Entities;

public sealed class PhrBatchStock : BaseEntity
{
    public long MedicineBatchId { get; set; }
    public decimal CurrentQty { get; set; }
    public decimal ReservedQty { get; set; }
    public decimal AvailableQty { get; set; }
    public decimal? ReorderLevelQty { get; set; }
    public DateTime? LastUpdatedOn { get; set; }
}