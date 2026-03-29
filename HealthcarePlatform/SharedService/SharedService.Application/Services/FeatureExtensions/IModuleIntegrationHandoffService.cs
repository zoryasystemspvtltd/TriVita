using Healthcare.Common.Responses;
using SharedService.Application.DTOs.FeatureExtensions;

namespace SharedService.Application.Services.FeatureExtensions;

public interface IModuleIntegrationHandoffService
{
    Task<BaseResponse<ModuleIntegrationHandoffResponseDto>> CreateAsync(
        CreateModuleIntegrationHandoffDto dto,
        CancellationToken cancellationToken = default);

    Task<BaseResponse<ModuleIntegrationHandoffResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<BaseResponse<IReadOnlyList<ModuleIntegrationHandoffResponseDto>>> ListByCorrelationAsync(
        string correlationId,
        CancellationToken cancellationToken = default);

    Task<BaseResponse<ModuleIntegrationHandoffResponseDto>> UpdateAsync(
        long id,
        UpdateModuleIntegrationHandoffDto dto,
        CancellationToken cancellationToken = default);

    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}
