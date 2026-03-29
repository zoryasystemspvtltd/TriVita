using Healthcare.Common.Responses;
using SharedService.Application.DTOs.FeatureExtensions;

namespace SharedService.Application.Services.FeatureExtensions;

public interface ITenantOnboardingStageService
{
    Task<BaseResponse<IReadOnlyList<TenantOnboardingStageResponseDto>>> ListAsync(CancellationToken cancellationToken = default);

    Task<BaseResponse<TenantOnboardingStageResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<BaseResponse<TenantOnboardingStageResponseDto>> GetByStageCodeAsync(
        string stageCode,
        CancellationToken cancellationToken = default);

    Task<BaseResponse<TenantOnboardingStageResponseDto>> UpsertAsync(
        UpsertTenantOnboardingStageDto dto,
        CancellationToken cancellationToken = default);

    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}
