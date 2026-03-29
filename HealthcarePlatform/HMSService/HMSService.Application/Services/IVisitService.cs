using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using HMSService.Application.DTOs.Visits;

namespace HMSService.Application.Services;

public interface IVisitService
{
    Task<BaseResponse<VisitResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<BaseResponse<PagedResponse<VisitResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? patientId,
        long? doctorId,
        DateTime? visitFrom,
        DateTime? visitTo,
        CancellationToken cancellationToken = default);

    Task<BaseResponse<VisitResponseDto>> CreateAsync(
        CreateVisitDto dto,
        CancellationToken cancellationToken = default);

    Task<BaseResponse<VisitResponseDto>> UpdateAsync(
        long id,
        UpdateVisitDto dto,
        CancellationToken cancellationToken = default);

    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}
