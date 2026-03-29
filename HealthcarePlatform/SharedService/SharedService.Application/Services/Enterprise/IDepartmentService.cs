using Healthcare.Common.Responses;
using SharedService.Application.DTOs.Enterprise;

namespace SharedService.Application.Services.Enterprise;

public interface IDepartmentService
{
    Task<BaseResponse<IReadOnlyList<DepartmentResponseDto>>> ListAsync(long? facilityId, CancellationToken cancellationToken = default);

    Task<BaseResponse<DepartmentResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<BaseResponse<DepartmentResponseDto>> CreateAsync(CreateDepartmentDto dto, CancellationToken cancellationToken = default);

    Task<BaseResponse<DepartmentResponseDto>> UpdateAsync(long id, UpdateDepartmentDto dto, CancellationToken cancellationToken = default);

    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}
