using Asp.Versioning;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using HMSService.Application.Integration;
using Microsoft.AspNetCore.Authorization;
using Healthcare.Common.Authorization;
using Healthcare.Common.Security;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace HMSService.API.Controllers.v1;

/// <summary>Proxies LMS lab invoice data for HMS billing visibility and payment tracking (loose coupling via HTTP).</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/integration/lab-billing")]
[RequirePermission(TriVitaPermissions.HmsApi)]
[SwaggerTag("HMS â€” LMS lab billing integration")]
public sealed class LabBillingIntegrationController : ControllerBase
{
    private readonly ILmsBillingClient _lmsBilling;
    private readonly ITenantContext _tenant;
    private readonly ILogger<LabBillingIntegrationController> _logger;

    public LabBillingIntegrationController(
        ILmsBillingClient lmsBilling,
        ITenantContext tenant,
        ILogger<LabBillingIntegrationController> logger)
    {
        _lmsBilling = lmsBilling;
        _tenant = tenant;
        _logger = logger;
    }

    /// <summary>Paged lab invoices from LMSService (tenant/facility forwarded).</summary>
    [HttpGet("invoices")]
    [SwaggerOperation(OperationId = "LabBilling_GetInvoicesFromLms")]
    [ProducesResponseType(typeof(BaseResponse<PagedResponse<LabInvoiceSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<PagedResponse<LabInvoiceSummaryDto>>>> GetLabInvoices(
        [FromQuery] PagedQuery query,
        CancellationToken ct)
    {
        _logger.LogInformation(
            "LabBillingIntegration GetLabInvoices tenant {TenantId} facility {FacilityId} page {Page}",
            _tenant.TenantId,
            _tenant.FacilityId,
            query.Page);

        var result = await _lmsBilling.GetLabInvoicesPagedAsync(query, ct);
        return Ok(result);
    }
}
