using Healthcare.Common.Responses;
using LISService.Application.DTOs.Analyzer;

namespace LISService.Application.Abstractions;

/// <summary>HTTP client to LMS workflow integration APIs (master data resolution).</summary>
public interface ILmsWorkflowApiClient
{
    Task<BaseResponse<LmsBarcodeResolutionClientDto>?> ResolveBarcodeAsync(string barcodeValue, CancellationToken cancellationToken = default);
}
