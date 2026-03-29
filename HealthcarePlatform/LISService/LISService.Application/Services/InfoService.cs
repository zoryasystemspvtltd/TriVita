using Healthcare.Common.Responses;
using LISService.Application.DTOs;

namespace LISService.Application.Services;

public sealed class InfoService : IInfoService
{
    public BaseResponse<InfoResponseDto> GetInfo() =>
        BaseResponse<InfoResponseDto>.Ok(new InfoResponseDto
        {
            Service = "LISService",
            Version = "1.0",
            Module = "LIS"
        });
}
