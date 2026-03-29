using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;

namespace HMSService.Application.Integration;

public interface ILmsBillingClient
{
    Task<BaseResponse<PagedResponse<LabInvoiceSummaryDto>>> GetLabInvoicesPagedAsync(
        PagedQuery query,
        CancellationToken cancellationToken = default);
}
