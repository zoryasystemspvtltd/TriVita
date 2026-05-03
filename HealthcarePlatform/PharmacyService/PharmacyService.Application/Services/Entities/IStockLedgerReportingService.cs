using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using PharmacyService.Application.DTOs.Entities;

namespace PharmacyService.Application.Services.Entities;

public interface IStockLedgerReportingService
{
    Task<BaseResponse<PagedResponse<StockLedgerDetailedRowDto>>> GetDetailedAsync(
        StockLedgerReportQuery query,
        CancellationToken cancellationToken = default);

    Task<BaseResponse<PagedResponse<StockLedgerSummaryReportRowDto>>> GetSummaryAsync(
        StockLedgerReportQuery query,
        CancellationToken cancellationToken = default);

    Task<BaseResponse<byte[]>> ExportDetailedExcelAsync(StockLedgerReportQuery filter, CancellationToken cancellationToken = default);

    Task<BaseResponse<byte[]>> ExportSummaryExcelAsync(StockLedgerReportQuery filter, CancellationToken cancellationToken = default);
}
