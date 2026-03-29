using Healthcare.Common.Responses;
using SharedService.Application.DTOs.FeatureExtensions;

namespace SharedService.Application.Services.FeatureExtensions;

public interface IFacilityServicePriceListLineService
{
    Task<BaseResponse<IReadOnlyList<FacilityServicePriceListLineResponseDto>>> ListByPriceListAsync(
        long facilityId,
        long priceListId,
        CancellationToken cancellationToken = default);

    Task<BaseResponse<FacilityServicePriceListLineResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<BaseResponse<FacilityServicePriceListLineResponseDto>> CreateAsync(
        CreateFacilityServicePriceListLineDto dto,
        CancellationToken cancellationToken = default);

    Task<BaseResponse<FacilityServicePriceListLineResponseDto>> UpdateAsync(
        long id,
        UpdateFacilityServicePriceListLineDto dto,
        CancellationToken cancellationToken = default);

    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}
