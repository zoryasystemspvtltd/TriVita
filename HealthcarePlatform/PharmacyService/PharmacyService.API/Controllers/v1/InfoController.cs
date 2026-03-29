using Asp.Versioning;
using Healthcare.Common.Responses;
using Microsoft.AspNetCore.Authorization;
using Healthcare.Common.Authorization;
using Healthcare.Common.Security;
using Microsoft.AspNetCore.Mvc;
using PharmacyService.Application.DTOs;
using PharmacyService.Application.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace PharmacyService.API.Controllers.v1;

/// <summary>Service metadata for the Pharmacy module.</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/info")]
[RequirePermission(TriVitaPermissions.PharmacyApi)]
[SwaggerTag("Info")]
public sealed class InfoController : ControllerBase
{
    private readonly IInfoService _infoService;

    public InfoController(IInfoService infoService)
    {
        _infoService = infoService;
    }

    /// <summary>Returns module name and version.</summary>
    [HttpGet]
    [SwaggerOperation(Summary = "Get Pharmacy service info", OperationId = "Pharmacy_Info_Get")]
    [SwaggerResponse(StatusCodes.Status200OK, "Service descriptor.", typeof(BaseResponse<InfoResponseDto>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(BaseResponse<InfoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<BaseResponse<InfoResponseDto>> Get() => Ok(_infoService.GetInfo());
}
