using Asp.Versioning;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using LMSService.Application.DTOs.Entities;
using LMSService.Application.Services.Entities;
using Microsoft.AspNetCore.Mvc;
using Healthcare.Common.Authorization;
using Healthcare.Common.Security;
using Swashbuckle.AspNetCore.Annotations;

namespace LMSService.API.Controllers.v1.Entities;

#region Referral

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/referral-doctor-profiles")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS — referral doctor profiles")]
public sealed class ReferralDoctorProfilesController : ControllerBase
{
    private readonly ILmsReferralDoctorProfileService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<ReferralDoctorProfilesController> _logger;

    public ReferralDoctorProfilesController(ILmsReferralDoctorProfileService service, ITenantContext tenant, ILogger<ReferralDoctorProfilesController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "ReferralDoctorProfiles_GetById")]
    public async Task<ActionResult<BaseResponse<ReferralDoctorProfileResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("ReferralDoctorProfile GetById {Id} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "ReferralDoctorProfiles_GetPaged")]
    public async Task<ActionResult<BaseResponse<PagedResponse<ReferralDoctorProfileResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<ReferralDoctorProfileResponseDto>>> Create([FromBody] CreateReferralDoctorProfileDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<ReferralDoctorProfileResponseDto>>> Update(long id, [FromBody] UpdateReferralDoctorProfileDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/referral-fee-rules")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS — referral fee rules")]
public sealed class ReferralFeeRulesController : ControllerBase
{
    private readonly ILmsReferralFeeRuleService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<ReferralFeeRulesController> _logger;

    public ReferralFeeRulesController(ILmsReferralFeeRuleService service, ITenantContext tenant, ILogger<ReferralFeeRulesController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "ReferralFeeRules_GetById")]
    public async Task<ActionResult<BaseResponse<ReferralFeeRuleResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("ReferralFeeRule GetById {Id} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "ReferralFeeRules_GetPaged")]
    public async Task<ActionResult<BaseResponse<PagedResponse<ReferralFeeRuleResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<ReferralFeeRuleResponseDto>>> Create([FromBody] CreateReferralFeeRuleDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<ReferralFeeRuleResponseDto>>> Update(long id, [FromBody] UpdateReferralFeeRuleDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/referral-fee-ledgers")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS — referral fee ledgers")]
public sealed class ReferralFeeLedgersController : ControllerBase
{
    private readonly ILmsReferralFeeLedgerService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<ReferralFeeLedgersController> _logger;

    public ReferralFeeLedgersController(ILmsReferralFeeLedgerService service, ITenantContext tenant, ILogger<ReferralFeeLedgersController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "ReferralFeeLedgers_GetById")]
    public async Task<ActionResult<BaseResponse<ReferralFeeLedgerResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("ReferralFeeLedger GetById {Id} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "ReferralFeeLedgers_GetPaged")]
    public async Task<ActionResult<BaseResponse<PagedResponse<ReferralFeeLedgerResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<ReferralFeeLedgerResponseDto>>> Create([FromBody] CreateReferralFeeLedgerDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<ReferralFeeLedgerResponseDto>>> Update(long id, [FromBody] UpdateReferralFeeLedgerDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/referral-settlements")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS — referral settlements")]
public sealed class ReferralSettlementsController : ControllerBase
{
    private readonly ILmsReferralSettlementService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<ReferralSettlementsController> _logger;

    public ReferralSettlementsController(ILmsReferralSettlementService service, ITenantContext tenant, ILogger<ReferralSettlementsController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "ReferralSettlements_GetById")]
    public async Task<ActionResult<BaseResponse<ReferralSettlementResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("ReferralSettlement GetById {Id} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "ReferralSettlements_GetPaged")]
    public async Task<ActionResult<BaseResponse<PagedResponse<ReferralSettlementResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<ReferralSettlementResponseDto>>> Create([FromBody] CreateReferralSettlementDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<ReferralSettlementResponseDto>>> Update(long id, [FromBody] UpdateReferralSettlementDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/referral-settlement-lines")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS — referral settlement lines")]
public sealed class ReferralSettlementLinesController : ControllerBase
{
    private readonly ILmsReferralSettlementLineService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<ReferralSettlementLinesController> _logger;

    public ReferralSettlementLinesController(ILmsReferralSettlementLineService service, ITenantContext tenant, ILogger<ReferralSettlementLinesController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "ReferralSettlementLines_GetById")]
    public async Task<ActionResult<BaseResponse<ReferralSettlementLineResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("ReferralSettlementLine GetById {Id} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "ReferralSettlementLines_GetPaged")]
    public async Task<ActionResult<BaseResponse<PagedResponse<ReferralSettlementLineResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<ReferralSettlementLineResponseDto>>> Create([FromBody] CreateReferralSettlementLineDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<ReferralSettlementLineResponseDto>>> Update(long id, [FromBody] UpdateReferralSettlementLineDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

#endregion

#region B2B

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/b2b-partners")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS — B2B partners")]
public sealed class B2BPartnersController : ControllerBase
{
    private readonly ILmsB2BPartnerService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<B2BPartnersController> _logger;

    public B2BPartnersController(ILmsB2BPartnerService service, ITenantContext tenant, ILogger<B2BPartnersController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "B2BPartners_GetById")]
    public async Task<ActionResult<BaseResponse<B2BPartnerResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("B2BPartner GetById {Id} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "B2BPartners_GetPaged")]
    public async Task<ActionResult<BaseResponse<PagedResponse<B2BPartnerResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<B2BPartnerResponseDto>>> Create([FromBody] CreateB2BPartnerDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<B2BPartnerResponseDto>>> Update(long id, [FromBody] UpdateB2BPartnerDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/b2b-partner-test-rates")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS — B2B partner test rates")]
public sealed class B2BPartnerTestRatesController : ControllerBase
{
    private readonly ILmsB2BPartnerTestRateService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<B2BPartnerTestRatesController> _logger;

    public B2BPartnerTestRatesController(ILmsB2BPartnerTestRateService service, ITenantContext tenant, ILogger<B2BPartnerTestRatesController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "B2BPartnerTestRates_GetById")]
    public async Task<ActionResult<BaseResponse<B2BPartnerTestRateResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("B2BPartnerTestRate GetById {Id} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "B2BPartnerTestRates_GetPaged")]
    public async Task<ActionResult<BaseResponse<PagedResponse<B2BPartnerTestRateResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<B2BPartnerTestRateResponseDto>>> Create([FromBody] CreateB2BPartnerTestRateDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<B2BPartnerTestRateResponseDto>>> Update(long id, [FromBody] UpdateB2BPartnerTestRateDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/b2b-partner-credit-profiles")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS — B2B partner credit profiles")]
public sealed class B2BPartnerCreditProfilesController : ControllerBase
{
    private readonly ILmsB2BPartnerCreditProfileService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<B2BPartnerCreditProfilesController> _logger;

    public B2BPartnerCreditProfilesController(ILmsB2BPartnerCreditProfileService service, ITenantContext tenant, ILogger<B2BPartnerCreditProfilesController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "B2BPartnerCreditProfiles_GetById")]
    public async Task<ActionResult<BaseResponse<B2BPartnerCreditProfileResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("B2BPartnerCreditProfile GetById {Id} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "B2BPartnerCreditProfiles_GetPaged")]
    public async Task<ActionResult<BaseResponse<PagedResponse<B2BPartnerCreditProfileResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<B2BPartnerCreditProfileResponseDto>>> Create([FromBody] CreateB2BPartnerCreditProfileDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<B2BPartnerCreditProfileResponseDto>>> Update(long id, [FromBody] UpdateB2BPartnerCreditProfileDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/b2b-credit-ledgers")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS — B2B credit ledgers")]
public sealed class B2BCreditLedgersController : ControllerBase
{
    private readonly ILmsB2BCreditLedgerService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<B2BCreditLedgersController> _logger;

    public B2BCreditLedgersController(ILmsB2BCreditLedgerService service, ITenantContext tenant, ILogger<B2BCreditLedgersController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "B2BCreditLedgers_GetById")]
    public async Task<ActionResult<BaseResponse<B2BCreditLedgerResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("B2BCreditLedger GetById {Id} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "B2BCreditLedgers_GetPaged")]
    public async Task<ActionResult<BaseResponse<PagedResponse<B2BCreditLedgerResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<B2BCreditLedgerResponseDto>>> Create([FromBody] CreateB2BCreditLedgerDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<B2BCreditLedgerResponseDto>>> Update(long id, [FromBody] UpdateB2BCreditLedgerDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/b2b-partner-billing-statements")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS — B2B partner billing statements")]
public sealed class B2BPartnerBillingStatementsController : ControllerBase
{
    private readonly ILmsB2BPartnerBillingStatementService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<B2BPartnerBillingStatementsController> _logger;

    public B2BPartnerBillingStatementsController(ILmsB2BPartnerBillingStatementService service, ITenantContext tenant, ILogger<B2BPartnerBillingStatementsController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "B2BPartnerBillingStatements_GetById")]
    public async Task<ActionResult<BaseResponse<B2BPartnerBillingStatementResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("B2BPartnerBillingStatement GetById {Id} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "B2BPartnerBillingStatements_GetPaged")]
    public async Task<ActionResult<BaseResponse<PagedResponse<B2BPartnerBillingStatementResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<B2BPartnerBillingStatementResponseDto>>> Create([FromBody] CreateB2BPartnerBillingStatementDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<B2BPartnerBillingStatementResponseDto>>> Update(long id, [FromBody] UpdateB2BPartnerBillingStatementDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/b2b-partner-billing-statement-lines")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS — B2B partner billing statement lines")]
public sealed class B2BPartnerBillingStatementLinesController : ControllerBase
{
    private readonly ILmsB2BPartnerBillingStatementLineService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<B2BPartnerBillingStatementLinesController> _logger;

    public B2BPartnerBillingStatementLinesController(ILmsB2BPartnerBillingStatementLineService service, ITenantContext tenant, ILogger<B2BPartnerBillingStatementLinesController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "B2BPartnerBillingStatementLines_GetById")]
    public async Task<ActionResult<BaseResponse<B2BPartnerBillingStatementLineResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("B2BPartnerBillingStatementLine GetById {Id} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "B2BPartnerBillingStatementLines_GetPaged")]
    public async Task<ActionResult<BaseResponse<PagedResponse<B2BPartnerBillingStatementLineResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<B2BPartnerBillingStatementLineResponseDto>>> Create([FromBody] CreateB2BPartnerBillingStatementLineDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<B2BPartnerBillingStatementLineResponseDto>>> Update(long id, [FromBody] UpdateB2BPartnerBillingStatementLineDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

#endregion

#region Reagent / catalog maps

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/reagent-masters")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS — reagent masters")]
public sealed class ReagentMastersController : ControllerBase
{
    private readonly ILmsReagentMasterService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<ReagentMastersController> _logger;

    public ReagentMastersController(ILmsReagentMasterService service, ITenantContext tenant, ILogger<ReagentMastersController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "ReagentMasters_GetById")]
    public async Task<ActionResult<BaseResponse<ReagentMasterResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("ReagentMaster GetById {Id} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "ReagentMasters_GetPaged")]
    public async Task<ActionResult<BaseResponse<PagedResponse<ReagentMasterResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<ReagentMasterResponseDto>>> Create([FromBody] CreateReagentMasterDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<ReagentMasterResponseDto>>> Update(long id, [FromBody] UpdateReagentMasterDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/reagent-batches")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS — reagent batches")]
public sealed class ReagentBatchesController : ControllerBase
{
    private readonly ILmsReagentBatchService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<ReagentBatchesController> _logger;

    public ReagentBatchesController(ILmsReagentBatchService service, ITenantContext tenant, ILogger<ReagentBatchesController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "ReagentBatches_GetById")]
    public async Task<ActionResult<BaseResponse<ReagentBatchResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("ReagentBatch GetById {Id} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "ReagentBatches_GetPaged")]
    public async Task<ActionResult<BaseResponse<PagedResponse<ReagentBatchResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<ReagentBatchResponseDto>>> Create([FromBody] CreateReagentBatchDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<ReagentBatchResponseDto>>> Update(long id, [FromBody] UpdateReagentBatchDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/test-reagent-maps")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS — test reagent maps")]
public sealed class TestReagentMapsController : ControllerBase
{
    private readonly ILmsTestReagentMapService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<TestReagentMapsController> _logger;

    public TestReagentMapsController(ILmsTestReagentMapService service, ITenantContext tenant, ILogger<TestReagentMapsController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "TestReagentMaps_GetById")]
    public async Task<ActionResult<BaseResponse<TestReagentMapResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("TestReagentMap GetById {Id} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "TestReagentMaps_GetPaged")]
    public async Task<ActionResult<BaseResponse<PagedResponse<TestReagentMapResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<TestReagentMapResponseDto>>> Create([FromBody] CreateTestReagentMapDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<TestReagentMapResponseDto>>> Update(long id, [FromBody] UpdateTestReagentMapDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

#endregion

#region Finance / gating / analytics / audit

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/report-payment-gates")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS — report payment gates")]
public sealed class ReportPaymentGatesController : ControllerBase
{
    private readonly ILmsReportPaymentGateService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<ReportPaymentGatesController> _logger;

    public ReportPaymentGatesController(ILmsReportPaymentGateService service, ITenantContext tenant, ILogger<ReportPaymentGatesController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "ReportPaymentGates_GetById")]
    public async Task<ActionResult<BaseResponse<ReportPaymentGateResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("ReportPaymentGate GetById {Id} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "ReportPaymentGates_GetPaged")]
    public async Task<ActionResult<BaseResponse<PagedResponse<ReportPaymentGateResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<ReportPaymentGateResponseDto>>> Create([FromBody] CreateReportPaymentGateDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<ReportPaymentGateResponseDto>>> Update(long id, [FromBody] UpdateReportPaymentGateDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/finance-ledger-entries")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS — finance ledger entries")]
public sealed class FinanceLedgerEntriesController : ControllerBase
{
    private readonly ILmsFinanceLedgerEntryService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<FinanceLedgerEntriesController> _logger;

    public FinanceLedgerEntriesController(ILmsFinanceLedgerEntryService service, ITenantContext tenant, ILogger<FinanceLedgerEntriesController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "FinanceLedgerEntries_GetById")]
    public async Task<ActionResult<BaseResponse<FinanceLedgerEntryResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("FinanceLedgerEntry GetById {Id} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "FinanceLedgerEntries_GetPaged")]
    public async Task<ActionResult<BaseResponse<PagedResponse<FinanceLedgerEntryResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<FinanceLedgerEntryResponseDto>>> Create([FromBody] CreateFinanceLedgerEntryDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<FinanceLedgerEntryResponseDto>>> Update(long id, [FromBody] UpdateFinanceLedgerEntryDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/analytics-daily-facility-rollups")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS — analytics daily facility rollups")]
public sealed class AnalyticsDailyFacilityRollupsController : ControllerBase
{
    private readonly ILmsAnalyticsDailyFacilityRollupService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<AnalyticsDailyFacilityRollupsController> _logger;

    public AnalyticsDailyFacilityRollupsController(ILmsAnalyticsDailyFacilityRollupService service, ITenantContext tenant, ILogger<AnalyticsDailyFacilityRollupsController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "AnalyticsDailyFacilityRollups_GetById")]
    public async Task<ActionResult<BaseResponse<AnalyticsDailyFacilityRollupResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("AnalyticsDailyFacilityRollup GetById {Id} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "AnalyticsDailyFacilityRollups_GetPaged")]
    public async Task<ActionResult<BaseResponse<PagedResponse<AnalyticsDailyFacilityRollupResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<AnalyticsDailyFacilityRollupResponseDto>>> Create([FromBody] CreateAnalyticsDailyFacilityRollupDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<AnalyticsDailyFacilityRollupResponseDto>>> Update(long id, [FromBody] UpdateAnalyticsDailyFacilityRollupDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/sec/data-change-audit-logs")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS — data change audit logs")]
public sealed class SecDataChangeAuditLogsController : ControllerBase
{
    private readonly ISecDataChangeAuditLogService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<SecDataChangeAuditLogsController> _logger;

    public SecDataChangeAuditLogsController(ISecDataChangeAuditLogService service, ITenantContext tenant, ILogger<SecDataChangeAuditLogsController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "SecDataChangeAuditLogs_GetById")]
    public async Task<ActionResult<BaseResponse<SecDataChangeAuditLogResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("SecDataChangeAuditLog GetById {Id} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "SecDataChangeAuditLogs_GetPaged")]
    public async Task<ActionResult<BaseResponse<PagedResponse<SecDataChangeAuditLogResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<SecDataChangeAuditLogResponseDto>>> Create([FromBody] CreateSecDataChangeAuditLogDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<SecDataChangeAuditLogResponseDto>>> Update(long id, [FromBody] UpdateSecDataChangeAuditLogDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

#endregion
