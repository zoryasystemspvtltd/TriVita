using Asp.Versioning;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using LMSService.Application.DTOs.Entities;
using LMSService.Application.Services.Entities;
using Microsoft.AspNetCore.Authorization;
using Healthcare.Common.Authorization;
using Healthcare.Common.Security;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LMSService.API.Controllers.v1.Entities;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/lab-invoice-headers")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS â€” lab billing (schema 07)")]
public sealed class LabInvoiceHeadersController : ControllerBase
{
    private readonly ILmsLabInvoiceHeaderService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<LabInvoiceHeadersController> _logger;

    public LabInvoiceHeadersController(
        ILmsLabInvoiceHeaderService service,
        ITenantContext tenant,
        ILogger<LabInvoiceHeadersController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<BaseResponse<LabInvoiceHeaderResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("LabInvoiceHeader GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    public async Task<ActionResult<BaseResponse<PagedResponse<LabInvoiceHeaderResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<LabInvoiceHeaderResponseDto>>> Create([FromBody] CreateLabInvoiceHeaderDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<LabInvoiceHeaderResponseDto>>> Update(long id, [FromBody] UpdateLabInvoiceHeaderDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/lab-order-contexts")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS â€” lab order context (B2B / referral / TAT)")]
public sealed class LabOrderContextsController : ControllerBase
{
    private readonly ILmsLabOrderContextService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<LabOrderContextsController> _logger;

    public LabOrderContextsController(
        ILmsLabOrderContextService service,
        ITenantContext tenant,
        ILogger<LabOrderContextsController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<BaseResponse<LabOrderContextResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("LabOrderContext GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    public async Task<ActionResult<BaseResponse<PagedResponse<LabOrderContextResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<LabOrderContextResponseDto>>> Create([FromBody] CreateLabOrderContextDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<LabOrderContextResponseDto>>> Update(long id, [FromBody] UpdateLabOrderContextDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/test-packages")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS â€” test packages")]
public sealed class TestPackages07Controller : ControllerBase
{
    private readonly ILmsTestPackageService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<TestPackages07Controller> _logger;

    public TestPackages07Controller(
        ILmsTestPackageService service,
        ITenantContext tenant,
        ILogger<TestPackages07Controller> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<BaseResponse<TestPackageResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("TestPackage GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    public async Task<ActionResult<BaseResponse<PagedResponse<TestPackageResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<TestPackageResponseDto>>> Create([FromBody] CreateTestPackageDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<TestPackageResponseDto>>> Update(long id, [FromBody] UpdateTestPackageDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/iam/user-accounts")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS â€” IAM user accounts (lab portal)")]
public sealed class IamUserAccountsController : ControllerBase
{
    private readonly IIamUserAccountService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<IamUserAccountsController> _logger;

    public IamUserAccountsController(
        IIamUserAccountService service,
        ITenantContext tenant,
        ILogger<IamUserAccountsController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<BaseResponse<IamUserAccountResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("IamUserAccount GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    public async Task<ActionResult<BaseResponse<PagedResponse<IamUserAccountResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<IamUserAccountResponseDto>>> Create([FromBody] CreateIamUserAccountDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<IamUserAccountResponseDto>>> Update(long id, [FromBody] UpdateIamUserAccountDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/reagent-consumption-logs")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS â€” reagent batch consumption")]
public sealed class ReagentConsumptionLogsController : ControllerBase
{
    private readonly ILmsReagentConsumptionLogService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<ReagentConsumptionLogsController> _logger;

    public ReagentConsumptionLogsController(
        ILmsReagentConsumptionLogService service,
        ITenantContext tenant,
        ILogger<ReagentConsumptionLogsController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<BaseResponse<ReagentConsumptionLogResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("ReagentConsumptionLog GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    public async Task<ActionResult<BaseResponse<PagedResponse<ReagentConsumptionLogResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<ReagentConsumptionLogResponseDto>>> Create([FromBody] CreateReagentConsumptionLogDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<ReagentConsumptionLogResponseDto>>> Update(long id, [FromBody] UpdateReagentConsumptionLogDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}
