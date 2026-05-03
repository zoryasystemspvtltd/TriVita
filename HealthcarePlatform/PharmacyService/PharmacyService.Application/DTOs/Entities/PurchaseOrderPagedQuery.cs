using Healthcare.Common.Pagination;

namespace PharmacyService.Application.DTOs.Entities;

public sealed class PurchaseOrderPagedQuery : PagedQuery
{
    public long? SupplierId { get; set; }
}
