using Healthcare.Common.Responses;
using PharmacyService.Application.DTOs;

namespace PharmacyService.Application.Services;

public sealed class InfoService : IInfoService
{
    public BaseResponse<InfoResponseDto> GetInfo() =>
        BaseResponse<InfoResponseDto>.Ok(new InfoResponseDto
        {
            Service = "PharmacyService",
            Version = "1.0",
            Module = "Pharmacy"
        });
}
