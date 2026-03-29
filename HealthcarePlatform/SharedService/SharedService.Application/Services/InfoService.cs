using Healthcare.Common.Responses;
using SharedService.Application.DTOs;

namespace SharedService.Application.Services;

public sealed class InfoService : IInfoService
{
    public BaseResponse<InfoResponseDto> GetInfo() =>
        BaseResponse<InfoResponseDto>.Ok(new InfoResponseDto
        {
            Service = "SharedService",
            Version = "1.0",
            Module = "Shared"
        });
}
