using Healthcare.Common.Entities;

namespace PharmacyService.Domain.Entities;

public sealed class PhrPurchaseOrder : BaseEntity
{
    public string PurchaseOrderNo { get; set; } = null!;
    public string SupplierName { get; set; } = null!;
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedOn { get; set; }
    public long StatusReferenceValueId { get; set; }
    public string? Notes { get; set; }
}