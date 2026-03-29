using Healthcare.Common.Responses;
using SharedService.Application.DTOs.FeatureExtensions;

namespace SharedService.Application.Services.FeatureExtensions;

public interface ILabCriticalValueEscalationService
{
    Task<BaseResponse<LabCriticalValueEscalationResponseDto>> CreateAsync(
        CreateLabCriticalValueEscalationDto dto,
        CancellationToken cancellationToken = default);

    Task<BaseResponse<LabCriticalValueEscalationResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<BaseResponse<IReadOnlyList<LabCriticalValueEscalationResponseDto>>> ListByLabResultAsync(
        long facilityId,
        long labResultId,
        CancellationToken cancellationToken = default);

    Task<BaseResponse<LabCriticalValueEscalationResponseDto>> UpdateAsync(
        long id,
        UpdateLabCriticalValueEscalationDto dto,
        CancellationToken cancellationToken = default);

    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}
