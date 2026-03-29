using Healthcare.Common.Responses;
using LMSService.Application.DTOs;

namespace LMSService.Application.Services;

public sealed class InfoService : IInfoService
{
    public BaseResponse<InfoResponseDto> GetInfo() =>
        BaseResponse<InfoResponseDto>.Ok(new InfoResponseDto
        {
            Service = "LMSService",
            Version = "1.0",
            Module = "LMS"
        });
}
