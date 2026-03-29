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
[Route("api/v{version:apiVersion}/facility-service-price-lists")]
[Authorize]
[RequirePermission(TriVitaPermissions.SharedApi)]
[SwaggerTag("09 — Facility service price lists")]
public sealed class FacilityServicePriceListsController : ControllerBase
{
    private readonly IFacilityServicePriceListService _service;

    public FacilityServicePriceListsController(IFacilityServicePriceListService service)
    {
        _service = service;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "List price lists for a facility", OperationId = "Shared_PriceLists_List")]
    [ProducesResponseType(typeof(BaseResponse<IReadOnlyList<FacilityServicePriceListResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<IReadOnlyList<FacilityServicePriceListResponseDto>>>> List(
        [FromQuery] long facilityId,
        CancellationToken cancellationToken)
    {
        var result = await _service.ListByFacilityAsync(facilityId, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Get price list by id", OperationId = "Shared_PriceLists_GetById")]
    [ProducesResponseType(typeof(BaseResponse<FacilityServicePriceListResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<FacilityServicePriceListResponseDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await _service.GetByIdAsync(id, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create price list", OperationId = "Shared_PriceLists_Create")]
    [ProducesResponseType(typeof(BaseResponse<FacilityServicePriceListResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<FacilityServicePriceListResponseDto>>> Create(
        [FromBody] CreateFacilityServicePriceListDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _service.CreateAsync(dto, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:long}")]
    [SwaggerOperation(Summary = "Update price list", OperationId = "Shared_PriceLists_Update")]
    [ProducesResponseType(typeof(BaseResponse<FacilityServicePriceListResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<FacilityServicePriceListResponseDto>>> Update(
        long id,
        [FromBody] UpdateFacilityServicePriceListDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _service.UpdateAsync(id, dto, cancellationToken);
        if (!result.Success && result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
            return NotFound(result);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:long}")]
    [SwaggerOperation(Summary = "Soft-delete price list", OperationId = "Shared_PriceLists_Delete")]
    [ProducesResponseType(typeof(BaseResponse<object?>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await _service.DeleteAsync(id, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
