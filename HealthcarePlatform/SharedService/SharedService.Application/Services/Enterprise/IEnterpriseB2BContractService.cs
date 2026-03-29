using Healthcare.Common.Responses;
using SharedService.Application.DTOs.Enterprise;

namespace SharedService.Application.Services.Enterprise;

public interface IEnterpriseB2BContractService
{
    Task<BaseResponse<IReadOnlyList<EnterpriseB2BContractResponseDto>>> ListByEnterpriseAsync(
        long enterpriseId,
        CancellationToken cancellationToken = default);

    Task<BaseResponse<EnterpriseB2BContractResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<BaseResponse<EnterpriseB2BContractResponseDto>> CreateAsync(
        CreateEnterpriseB2BContractDto dto,
        CancellationToken cancellationToken = default);

    Task<BaseResponse<EnterpriseB2BContractResponseDto>> UpdateAsync(
        long id,
        UpdateEnterpriseB2BContractDto dto,
        CancellationToken cancellationToken = default);

    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}
