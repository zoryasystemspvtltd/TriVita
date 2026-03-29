using Healthcare.Common.Responses;
using SharedService.Application.DTOs.FeatureExtensions;

namespace SharedService.Application.Services.FeatureExtensions;

public interface IFacilityServicePriceListService
{
    Task<BaseResponse<IReadOnlyList<FacilityServicePriceListResponseDto>>> ListByFacilityAsync(
        long facilityId,
        CancellationToken cancellationToken = default);

    Task<BaseResponse<FacilityServicePriceListResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<BaseResponse<FacilityServicePriceListResponseDto>> CreateAsync(
        CreateFacilityServicePriceListDto dto,
        CancellationToken cancellationToken = default);

    Task<BaseResponse<FacilityServicePriceListResponseDto>> UpdateAsync(
        long id,
        UpdateFacilityServicePriceListDto dto,
        CancellationToken cancellationToken = default);

    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}
