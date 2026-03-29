using Healthcare.Common.Responses;
using PharmacyService.Application.DTOs;

namespace PharmacyService.Application.Services;

public interface IInfoService
{
    BaseResponse<InfoResponseDto> GetInfo();
}
