using Asp.Versioning;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using LISService.Application.DTOs.Entities;
using LISService.Application.Services.Entities;
using Microsoft.AspNetCore.Authorization;
using Healthcare.Common.Authorization;
using Healthcare.Common.Security;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LISService.API.Controllers.v1.Entities;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/test-parameter-profiles")]
[RequirePermission(TriVitaPermissions.LisApi)]
[SwaggerTag("LIS â€” test parameter profile (method / analyzer mapping)")]
public sealed class TestParameterProfileController : ControllerBase
{
    private readonly ILisTestParameterProfileService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<TestParameterProfileController> _logger;

    public TestParameterProfileController(
        ILisTestParameterProfileService service,
        ITenantContext tenant,
        ILogger<TestParameterProfileController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "TestParameterProfile_GetById")]
    public async Task<ActionResult<BaseResponse<TestParameterProfileResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("TestParameterProfile GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "TestParameterProfile_GetPaged")]
    public async Task<ActionResult<BaseResponse<PagedResponse<TestParameterProfileResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<TestParameterProfileResponseDto>>> Create([FromBody] CreateTestParameterProfileDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<TestParameterProfileResponseDto>>> Update(long id, [FromBody] UpdateTestParameterProfileDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/analyzer-result-maps")]
[RequirePermission(TriVitaPermissions.LisApi)]
[SwaggerTag("LIS â€” analyzer / LIS code mapping")]
public sealed class AnalyzerResultMapController : ControllerBase
{
    private readonly ILisAnalyzerResultMapService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<AnalyzerResultMapController> _logger;

    public AnalyzerResultMapController(
        ILisAnalyzerResultMapService service,
        ITenantContext tenant,
        ILogger<AnalyzerResultMapController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<BaseResponse<AnalyzerResultMapResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("AnalyzerResultMap GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    public async Task<ActionResult<BaseResponse<PagedResponse<AnalyzerResultMapResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<AnalyzerResultMapResponseDto>>> Create([FromBody] CreateAnalyzerResultMapDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<AnalyzerResultMapResponseDto>>> Update(long id, [FromBody] UpdateAnalyzerResultMapDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/sample-barcodes")]
[RequirePermission(TriVitaPermissions.LisApi)]
[SwaggerTag("LIS â€” sample barcode / QR")]
public sealed class SampleBarcodeController : ControllerBase
{
    private readonly ILisSampleBarcodeService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<SampleBarcodeController> _logger;

    public SampleBarcodeController(
        ILisSampleBarcodeService service,
        ITenantContext tenant,
        ILogger<SampleBarcodeController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<BaseResponse<SampleBarcodeResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("SampleBarcode GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    public async Task<ActionResult<BaseResponse<PagedResponse<SampleBarcodeResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<SampleBarcodeResponseDto>>> Create([FromBody] CreateSampleBarcodeDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<SampleBarcodeResponseDto>>> Update(long id, [FromBody] UpdateSampleBarcodeDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/sample-lifecycle-events")]
[RequirePermission(TriVitaPermissions.LisApi)]
[SwaggerTag("LIS â€” sample lifecycle / TAT milestones")]
public sealed class SampleLifecycleEventController : ControllerBase
{
    private readonly ILisSampleLifecycleEventService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<SampleLifecycleEventController> _logger;

    public SampleLifecycleEventController(
        ILisSampleLifecycleEventService service,
        ITenantContext tenant,
        ILogger<SampleLifecycleEventController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<BaseResponse<SampleLifecycleEventResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("SampleLifecycleEvent GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    public async Task<ActionResult<BaseResponse<PagedResponse<SampleLifecycleEventResponseDto>>>> GetPaged(
        [FromQuery] PagedQuery query,
        [FromQuery] long? sampleCollectionId,
        CancellationToken ct)
    {
        _logger.LogInformation("SampleLifecycleEvent GetPaged tenant {TenantId} sampleCollectionId {SampleCollectionId}", _tenant.TenantId, sampleCollectionId);
        return Ok(await _service.GetPagedAsync(query, sampleCollectionId, ct));
    }

    [HttpPost]
    public async Task<ActionResult<BaseResponse<SampleLifecycleEventResponseDto>>> Create([FromBody] CreateSampleLifecycleEventDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<SampleLifecycleEventResponseDto>>> Update(long id, [FromBody] UpdateSampleLifecycleEventDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/report-delivery-otps")]
[RequirePermission(TriVitaPermissions.LisApi)]
[SwaggerTag("LIS â€” OTP for report delivery")]
public sealed class ReportDeliveryOtpController : ControllerBase
{
    private readonly ILisReportDeliveryOtpService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<ReportDeliveryOtpController> _logger;

    public ReportDeliveryOtpController(
        ILisReportDeliveryOtpService service,
        ITenantContext tenant,
        ILogger<ReportDeliveryOtpController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<BaseResponse<ReportDeliveryOtpResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("ReportDeliveryOtp GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    public async Task<ActionResult<BaseResponse<PagedResponse<ReportDeliveryOtpResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<ReportDeliveryOtpResponseDto>>> Create([FromBody] CreateReportDeliveryOtpDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<ReportDeliveryOtpResponseDto>>> Update(long id, [FromBody] UpdateReportDeliveryOtpDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/report-lock-states")]
[RequirePermission(TriVitaPermissions.LisApi)]
[SwaggerTag("LIS â€” report lock / immutability")]
public sealed class ReportLockStateController : ControllerBase
{
    private readonly ILisReportLockStateService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<ReportLockStateController> _logger;

    public ReportLockStateController(
        ILisReportLockStateService service,
        ITenantContext tenant,
        ILogger<ReportLockStateController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<BaseResponse<ReportLockStateResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("ReportLockState GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    public async Task<ActionResult<BaseResponse<PagedResponse<ReportLockStateResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<ReportLockStateResponseDto>>> Create([FromBody] CreateReportLockStateDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<ReportLockStateResponseDto>>> Update(long id, [FromBody] UpdateReportLockStateDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}
