using Healthcare.Common.Responses;
using LISService.Application.DTOs;

namespace LISService.Application.Services;

public interface IInfoService
{
    BaseResponse<InfoResponseDto> GetInfo();
}
