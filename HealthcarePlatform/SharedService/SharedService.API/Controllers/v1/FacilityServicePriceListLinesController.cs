using Asp.Versioning;
using Healthcare.Common.Authorization;
using Healthcare.Common.Responses;
using Healthcare.Common.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedService.Application.DTOs.FeatureExtensions;
using SharedService.Application.Services.FeatureExtensions;
using Swashbuckle.AspNetCore.Annotations;

namespace SharedService.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/facility-service-price-list-lines")]
[Authorize]
[RequirePermission(TriVitaPermissions.SharedApi)]
[SwaggerTag("09 — Facility service price list lines")]
public sealed class FacilityServicePriceListLinesController : ControllerBase
{
    private readonly IFacilityServicePriceListLineService _service;

    public FacilityServicePriceListLinesController(IFacilityServicePriceListLineService service)
    {
        _service = service;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "List lines for a price list", OperationId = "Shared_PriceListLines_List")]
    [ProducesResponseType(typeof(BaseResponse<IReadOnlyList<FacilityServicePriceListLineResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<IReadOnlyList<FacilityServicePriceListLineResponseDto>>>> List(
        [FromQuery] long facilityId,
        [FromQuery] long priceListId,
        CancellationToken cancellationToken)
    {
        var result = await _service.ListByPriceListAsync(facilityId, priceListId, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Get line by id", OperationId = "Shared_PriceListLines_GetById")]
    [ProducesResponseType(typeof(BaseResponse<FacilityServicePriceListLineResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<FacilityServicePriceListLineResponseDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create line", OperationId = "Shared_PriceListLines_Create")]
    [ProducesResponseType(typeof(BaseResponse<FacilityServicePriceListLineResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<FacilityServicePriceListLineResponseDto>>> Create(
        [FromBody] CreateFacilityServicePriceListLineDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(dto, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:long}")]
    [SwaggerOperation(Summary = "Update line", OperationId = "Shared_PriceListLines_Update")]
    [ProducesResponseType(typeof(BaseResponse<FacilityServicePriceListLineResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<FacilityServicePriceListLineResponseDto>>> Update(
        long id,
        [FromBody] UpdateFacilityServicePriceListLineDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(id, dto, cancellationToken);
        if (!result.Success && result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
            return NotFound(result);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:long}")]
    [SwaggerOperation(Summary = "Soft-delete line", OperationId = "Shared_PriceListLines_Delete")]
    [ProducesResponseType(typeof(BaseResponse<object?>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(id, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
