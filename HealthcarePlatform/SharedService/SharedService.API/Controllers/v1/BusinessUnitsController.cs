using Asp.Versioning;
using Healthcare.Common.Authorization;
using Healthcare.Common.Responses;
using Healthcare.Common.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedService.Application.DTOs.Enterprise;
using SharedService.Application.Services.Enterprise;
using Swashbuckle.AspNetCore.Annotations;

namespace SharedService.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/business-units")]
[Authorize]
[RequirePermission(TriVitaPermissions.SharedApi)]
[SwaggerTag("Enterprise hierarchy")]
public sealed class BusinessUnitsController : ControllerBase
{
    private readonly IBusinessUnitService _businessUnitService;

    public BusinessUnitsController(IBusinessUnitService businessUnitService)
    {
        _businessUnitService = businessUnitService;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "List business units (optional company filter)", OperationId = "Shared_BusinessUnits_List")]
    [ProducesResponseType(typeof(BaseResponse<IReadOnlyList<BusinessUnitResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<IReadOnlyList<BusinessUnitResponseDto>>>> List(
        [FromQuery] long? companyId,
        CancellationToken cancellationToken) =>
        Ok(await _businessUnitService.ListAsync(companyId, cancellationToken));

    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Get business unit by id", OperationId = "Shared_BusinessUnits_GetById")]
    [ProducesResponseType(typeof(BaseResponse<BusinessUnitResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<BusinessUnitResponseDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await _businessUnitService.GetByIdAsync(id, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create business unit", OperationId = "Shared_BusinessUnits_Create")]
    [ProducesResponseType(typeof(BaseResponse<BusinessUnitResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<BusinessUnitResponseDto>>> Create(
        [FromBody] CreateBusinessUnitDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _businessUnitService.CreateAsync(dto, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:long}")]
    [SwaggerOperation(Summary = "Update business unit", OperationId = "Shared_BusinessUnits_Update")]
    [ProducesResponseType(typeof(BaseResponse<BusinessUnitResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<BusinessUnitResponseDto>>> Update(
        long id,
        [FromBody] UpdateBusinessUnitDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _businessUnitService.UpdateAsync(id, dto, cancellationToken);
        if (!result.Success && result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
            return NotFound(result);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:long}")]
    [SwaggerOperation(Summary = "Soft-delete business unit", OperationId = "Shared_BusinessUnits_Delete")]
    [ProducesResponseType(typeof(BaseResponse<object?>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await _businessUnitService.DeleteAsync(id, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
