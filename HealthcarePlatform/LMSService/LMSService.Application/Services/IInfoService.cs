using Healthcare.Common.Responses;
using LMSService.Application.DTOs;

namespace LMSService.Application.Services;

public interface IInfoService
{
    BaseResponse<InfoResponseDto> GetInfo();
}
