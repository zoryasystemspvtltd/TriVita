using Healthcare.Common.Responses;
using SharedService.Application.DTOs.Enterprise;

namespace SharedService.Application.Services.Enterprise;

public interface ICompanyService
{
    Task<BaseResponse<IReadOnlyList<CompanyResponseDto>>> ListAsync(long? enterpriseId, CancellationToken cancellationToken = default);

    Task<BaseResponse<CompanyResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<BaseResponse<CompanyResponseDto>> CreateAsync(CreateCompanyDto dto, CancellationToken cancellationToken = default);

    Task<BaseResponse<CompanyResponseDto>> UpdateAsync(long id, UpdateCompanyDto dto, CancellationToken cancellationToken = default);

    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}
