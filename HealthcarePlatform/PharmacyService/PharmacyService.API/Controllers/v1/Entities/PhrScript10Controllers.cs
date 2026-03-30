using Asp.Versioning;
using Healthcare.Common.Authorization;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using Healthcare.Common.Security;
using PharmacyService.Application.DTOs.Entities;
using PharmacyService.Application.Services.Entities;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace PharmacyService.API.Controllers.v1.Entities;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/inventory-locations")]
[RequirePermission(TriVitaPermissions.PharmacyApi)]
[SwaggerTag("Pharmacy inventory locations (script 10)")]
public sealed class InventoryLocationsController : ControllerBase
{
    private readonly IPhrInventoryLocationService _service;

    public InventoryLocationsController(IPhrInventoryLocationService service) => _service = service;

    [HttpGet("{id:long}")]
    public async Task<ActionResult<BaseResponse<PhrInventoryLocationResponseDto>>> GetById(long id, CancellationToken ct) =>
        Ok(await _service.GetByIdAsync(id, ct));

    [HttpGet]
    public async Task<ActionResult<BaseResponse<PagedResponse<PhrInventoryLocationResponseDto>>>> GetPaged(
        [FromQuery] PagedQuery query,
        CancellationToken ct) =>
        Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<PhrInventoryLocationResponseDto>>> Create(
        [FromBody] CreatePhrInventoryLocationDto dto,
        CancellationToken ct) =>
        Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<PhrInventoryLocationResponseDto>>> Update(
        long id,
        [FromBody] UpdatePhrInventoryLocationDto dto,
        CancellationToken ct) =>
        Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct) =>
        Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/sales-returns")]
[RequirePermission(TriVitaPermissions.PharmacyApi)]
public sealed class SalesReturnsController : ControllerBase
{
    private readonly IPhrSalesReturnService _service;

    public SalesReturnsController(IPhrSalesReturnService service) => _service = service;

    [HttpGet("{id:long}")]
    public async Task<ActionResult<BaseResponse<PhrSalesReturnResponseDto>>> GetById(long id, CancellationToken ct) =>
        Ok(await _service.GetByIdAsync(id, ct));

    [HttpGet]
    public async Task<ActionResult<BaseResponse<PagedResponse<PhrSalesReturnResponseDto>>>> GetPaged(
        [FromQuery] PagedQuery query,
        CancellationToken ct) =>
        Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<PhrSalesReturnResponseDto>>> Create(
        [FromBody] CreatePhrSalesReturnDto dto,
        CancellationToken ct) =>
        Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<PhrSalesReturnResponseDto>>> Update(
        long id,
        [FromBody] UpdatePhrSalesReturnDto dto,
        CancellationToken ct) =>
        Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct) =>
        Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/sales-return-items")]
[RequirePermission(TriVitaPermissions.PharmacyApi)]
public sealed class SalesReturnItemsController : ControllerBase
{
    private readonly IPhrSalesReturnItemService _service;

    public SalesReturnItemsController(IPhrSalesReturnItemService service) => _service = service;

    [HttpGet("{id:long}")]
    public async Task<ActionResult<BaseResponse<PhrSalesReturnItemResponseDto>>> GetById(long id, CancellationToken ct) =>
        Ok(await _service.GetByIdAsync(id, ct));

    [HttpGet]
    public async Task<ActionResult<BaseResponse<PagedResponse<PhrSalesReturnItemResponseDto>>>> GetPaged(
        [FromQuery] PagedQuery query,
        [FromQuery] long? salesReturnId,
        CancellationToken ct) =>
        Ok(await _service.GetPagedAsync(query, salesReturnId, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<PhrSalesReturnItemResponseDto>>> Create(
        [FromBody] CreatePhrSalesReturnItemDto dto,
        CancellationToken ct) =>
        Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<PhrSalesReturnItemResponseDto>>> Update(
        long id,
        [FromBody] UpdatePhrSalesReturnItemDto dto,
        CancellationToken ct) =>
        Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct) =>
        Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/controlled-drug-register")]
[RequirePermission(TriVitaPermissions.PharmacyApi)]
public sealed class ControlledDrugRegisterController : ControllerBase
{
    private readonly IPhrControlledDrugRegisterService _service;

    public ControlledDrugRegisterController(IPhrControlledDrugRegisterService service) => _service = service;

    [HttpGet("{id:long}")]
    public async Task<ActionResult<BaseResponse<PhrControlledDrugRegisterResponseDto>>> GetById(long id, CancellationToken ct) =>
        Ok(await _service.GetByIdAsync(id, ct));

    [HttpGet]
    public async Task<ActionResult<BaseResponse<PagedResponse<PhrControlledDrugRegisterResponseDto>>>> GetPaged(
        [FromQuery] PagedQuery query,
        CancellationToken ct) =>
        Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<PhrControlledDrugRegisterResponseDto>>> Create(
        [FromBody] CreatePhrControlledDrugRegisterDto dto,
        CancellationToken ct) =>
        Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<PhrControlledDrugRegisterResponseDto>>> Update(
        long id,
        [FromBody] UpdatePhrControlledDrugRegisterDto dto,
        CancellationToken ct) =>
        Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct) =>
        Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/batch-stock-locations")]
[RequirePermission(TriVitaPermissions.PharmacyApi)]
public sealed class BatchStockLocationsController : ControllerBase
{
    private readonly IPhrBatchStockLocationService _service;

    public BatchStockLocationsController(IPhrBatchStockLocationService service) => _service = service;

    [HttpGet("{id:long}")]
    public async Task<ActionResult<BaseResponse<PhrBatchStockLocationResponseDto>>> GetById(long id, CancellationToken ct) =>
        Ok(await _service.GetByIdAsync(id, ct));

    [HttpGet]
    public async Task<ActionResult<BaseResponse<PagedResponse<PhrBatchStockLocationResponseDto>>>> GetPaged(
        [FromQuery] PagedQuery query,
        [FromQuery] long? batchStockId,
        [FromQuery] long? inventoryLocationId,
        CancellationToken ct) =>
        Ok(await _service.GetPagedAsync(query, batchStockId, inventoryLocationId, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<PhrBatchStockLocationResponseDto>>> Create(
        [FromBody] CreatePhrBatchStockLocationDto dto,
        CancellationToken ct) =>
        Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<PhrBatchStockLocationResponseDto>>> Update(
        long id,
        [FromBody] UpdatePhrBatchStockLocationDto dto,
        CancellationToken ct) =>
        Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct) =>
        Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/reorder-policies")]
[RequirePermission(TriVitaPermissions.PharmacyApi)]
public sealed class ReorderPoliciesController : ControllerBase
{
    private readonly IPhrReorderPolicyService _service;

    public ReorderPoliciesController(IPhrReorderPolicyService service) => _service = service;

    [HttpGet("{id:long}")]
    public async Task<ActionResult<BaseResponse<PhrReorderPolicyResponseDto>>> GetById(long id, CancellationToken ct) =>
        Ok(await _service.GetByIdAsync(id, ct));

    [HttpGet]
    public async Task<ActionResult<BaseResponse<PagedResponse<PhrReorderPolicyResponseDto>>>> GetPaged(
        [FromQuery] PagedQuery query,
        [FromQuery] long? batchStockId,
        CancellationToken ct) =>
        Ok(await _service.GetPagedAsync(query, batchStockId, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<PhrReorderPolicyResponseDto>>> Create(
        [FromBody] CreatePhrReorderPolicyDto dto,
        CancellationToken ct) =>
        Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<PhrReorderPolicyResponseDto>>> Update(
        long id,
        [FromBody] UpdatePhrReorderPolicyDto dto,
        CancellationToken ct) =>
        Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct) =>
        Ok(await _service.DeleteAsync(id, ct));
}
