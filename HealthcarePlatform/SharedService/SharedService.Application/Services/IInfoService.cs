using Healthcare.Common.Responses;
using SharedService.Application.DTOs;

namespace SharedService.Application.Services;

public interface IInfoService
{
    BaseResponse<InfoResponseDto> GetInfo();
}
