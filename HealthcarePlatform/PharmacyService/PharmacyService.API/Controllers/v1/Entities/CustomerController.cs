using Asp.Versioning;
using Healthcare.Common.Authorization;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using Healthcare.Common.Security;
using Microsoft.AspNetCore.Mvc;
using PharmacyService.Application.DTOs.Entities;
using PharmacyService.Application.Services.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace PharmacyService.API.Controllers.v1.Entities;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/customer")]
[RequirePermission(TriVitaPermissions.PharmacyApi)]
[SwaggerTag("PhrCustomer")]
public sealed class CustomerController : ControllerBase
{
    private readonly IPhrCustomerService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<CustomerController> _logger;

    public CustomerController(IPhrCustomerService service, ITenantContext tenant, ILogger<CustomerController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "Customer_GetById")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<CustomerResponseDto>))]
    public async Task<ActionResult<BaseResponse<CustomerResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "Customer_GetPaged")]
    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<CustomerResponseDto>>))]
    public async Task<ActionResult<BaseResponse<PagedResponse<CustomerResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    [SwaggerOperation(OperationId = "Customer_Create")]
    public async Task<ActionResult<BaseResponse<CustomerResponseDto>>> Create([FromBody] CreateCustomerDto dto, CancellationToken ct)
    {
        var res = await _service.CreateAsync(dto, ct);
        if (res.Success) return Ok(res);
        if (res.Message is "Customer already exists with same mobile number." or "Customer already exists with same Aadhaar number.")
            return Conflict(res);
        return BadRequest(res);
    }

    [HttpPut("{id:long}")]
    [SwaggerOperation(OperationId = "Customer_Update")]
    public async Task<ActionResult<BaseResponse<CustomerResponseDto>>> Update(long id, [FromBody] UpdateCustomerDto dto, CancellationToken ct)
    {
        var res = await _service.UpdateAsync(id, dto, ct);
        if (res.Success) return Ok(res);
        if (res.Message is "Customer already exists with same mobile number." or "Customer already exists with same Aadhaar number.")
            return Conflict(res);
        return BadRequest(res);
    }

    [HttpDelete("{id:long}")]
    [SwaggerOperation(OperationId = "Customer_Delete")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

