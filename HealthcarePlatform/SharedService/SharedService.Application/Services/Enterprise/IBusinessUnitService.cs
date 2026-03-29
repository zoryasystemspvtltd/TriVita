using Healthcare.Common.Responses;
using SharedService.Application.DTOs.Enterprise;

namespace SharedService.Application.Services.Enterprise;

public interface IBusinessUnitService
{
    Task<BaseResponse<IReadOnlyList<BusinessUnitResponseDto>>> ListAsync(long? companyId, CancellationToken cancellationToken = default);

    Task<BaseResponse<BusinessUnitResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<BaseResponse<BusinessUnitResponseDto>> CreateAsync(CreateBusinessUnitDto dto, CancellationToken cancellationToken = default);

    Task<BaseResponse<BusinessUnitResponseDto>> UpdateAsync(long id, UpdateBusinessUnitDto dto, CancellationToken cancellationToken = default);

    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}
