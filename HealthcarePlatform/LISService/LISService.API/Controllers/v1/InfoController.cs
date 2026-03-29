using Asp.Versioning;
using Healthcare.Common.Responses;
using LISService.Application.DTOs;
using LISService.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Healthcare.Common.Authorization;
using Healthcare.Common.Security;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LISService.API.Controllers.v1;

/// <summary>Service metadata for discovery and health of the LIS module.</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/info")]
[RequirePermission(TriVitaPermissions.LisApi)]
[SwaggerTag("Info")]
public sealed class InfoController : ControllerBase
{
    private readonly IInfoService _infoService;

    public InfoController(IInfoService infoService)
    {
        _infoService = infoService;
    }

    /// <summary>Returns module name and version (wrapped in <see cref="BaseResponse{T}"/>).</summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Get LIS service info", OperationId = "Lis_Info_Get")]
    [SwaggerResponse(StatusCodes.Status200OK, "Service descriptor.", typeof(BaseResponse<InfoResponseDto>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<InfoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<BaseResponse<InfoResponseDto>> Get() => Ok(_infoService.GetInfo());
}
