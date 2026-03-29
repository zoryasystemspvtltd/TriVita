using Healthcare.Common.Responses;
using SharedService.Application.DTOs.Enterprise;

namespace SharedService.Application.Services.Enterprise;

public interface IEnterpriseService
{
    Task<BaseResponse<IReadOnlyList<EnterpriseResponseDto>>> ListAsync(CancellationToken cancellationToken = default);

    Task<BaseResponse<EnterpriseResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<BaseResponse<EnterpriseResponseDto>> CreateAsync(CreateEnterpriseDto dto, CancellationToken cancellationToken = default);

    Task<BaseResponse<EnterpriseResponseDto>> UpdateAsync(long id, UpdateEnterpriseDto dto, CancellationToken cancellationToken = default);

    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);

    Task<BaseResponse<EnterpriseHierarchyResponseDto>> GetHierarchyAsync(long enterpriseId, CancellationToken cancellationToken = default);
}
