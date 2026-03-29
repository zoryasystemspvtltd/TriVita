using Healthcare.Common.Entities;

namespace PharmacyService.Domain.Entities;

public sealed class PhrStockTransfer : BaseEntity
{
    public string TransferNo { get; set; } = null!;
    public long FromFacilityId { get; set; }
    public long ToFacilityId { get; set; }
    public DateTime TransferOn { get; set; }
    public long StatusReferenceValueId { get; set; }
    public string? Notes { get; set; }
}