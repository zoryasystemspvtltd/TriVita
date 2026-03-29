using Healthcare.Common.Responses;
using SharedService.Application.DTOs.Enterprise;

namespace SharedService.Application.Services.Enterprise;

public interface IFacilityService
{
    Task<BaseResponse<IReadOnlyList<FacilityResponseDto>>> ListAsync(long? businessUnitId, CancellationToken cancellationToken = default);

    Task<BaseResponse<FacilityResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<BaseResponse<FacilityHierarchyContextDto>> GetHierarchyContextAsync(long facilityId, CancellationToken cancellationToken = default);

    Task<BaseResponse<FacilityResponseDto>> CreateAsync(CreateFacilityDto dto, CancellationToken cancellationToken = default);

    Task<BaseResponse<FacilityResponseDto>> UpdateAsync(long id, UpdateFacilityDto dto, CancellationToken cancellationToken = default);

    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}
