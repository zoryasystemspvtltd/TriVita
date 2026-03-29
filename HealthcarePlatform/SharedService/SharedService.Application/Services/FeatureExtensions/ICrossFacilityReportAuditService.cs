using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using SharedService.Application.DTOs.FeatureExtensions;

namespace SharedService.Application.Services.FeatureExtensions;

public interface ICrossFacilityReportAuditService
{
    Task<BaseResponse<CrossFacilityReportAuditResponseDto>> CreateAsync(
        CreateCrossFacilityReportAuditDto dto,
        CancellationToken cancellationToken = default);

    Task<BaseResponse<CrossFacilityReportAuditResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<BaseResponse<PagedResponse<CrossFacilityReportAuditResponseDto>>> GetPagedAsync(
        PagedQuery query,
        string? reportCode,
        CancellationToken cancellationToken = default);

    Task<BaseResponse<CrossFacilityReportAuditResponseDto>> UpdateAsync(
        long id,
        UpdateCrossFacilityReportAuditDto dto,
        CancellationToken cancellationToken = default);

    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}
