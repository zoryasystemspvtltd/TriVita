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

#region IAM (RequiresFacilityId = false in services)

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/iam/roles")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS IAM — roles")]
public sealed class IamRolesController : ControllerBase
{
    private readonly IIamRoleService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<IamRolesController> _logger;

    public IamRolesController(IIamRoleService service, ITenantContext tenant, ILogger<IamRolesController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "IamRoles_GetById")]
    public async Task<ActionResult<BaseResponse<IamRoleResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("IamRole GetById {Id} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "IamRoles_GetPaged")]
    public async Task<ActionResult<BaseResponse<PagedResponse<IamRoleResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<IamRoleResponseDto>>> Create([FromBody] CreateIamRoleDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<IamRoleResponseDto>>> Update(long id, [FromBody] UpdateIamRoleDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/iam/permissions")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS IAM — permissions")]
public sealed class IamPermissionsController : ControllerBase
{
    private readonly IIamPermissionService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<IamPermissionsController> _logger;

    public IamPermissionsController(IIamPermissionService service, ITenantContext tenant, ILogger<IamPermissionsController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "IamPermissions_GetById")]
    public async Task<ActionResult<BaseResponse<IamPermissionResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("IamPermission GetById {Id} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "IamPermissions_GetPaged")]
    public async Task<ActionResult<BaseResponse<PagedResponse<IamPermissionResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<IamPermissionResponseDto>>> Create([FromBody] CreateIamPermissionDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<IamPermissionResponseDto>>> Update(long id, [FromBody] UpdateIamPermissionDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/iam/role-permissions")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS IAM — role permissions")]
public sealed class IamRolePermissionsController : ControllerBase
{
    private readonly IIamRolePermissionService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<IamRolePermissionsController> _logger;

    public IamRolePermissionsController(IIamRolePermissionService service, ITenantContext tenant, ILogger<IamRolePermissionsController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "IamRolePermissions_GetById")]
    public async Task<ActionResult<BaseResponse<IamRolePermissionResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("IamRolePermission GetById {Id} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "IamRolePermissions_GetPaged")]
    public async Task<ActionResult<BaseResponse<PagedResponse<IamRolePermissionResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<IamRolePermissionResponseDto>>> Create([FromBody] CreateIamRolePermissionDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<IamRolePermissionResponseDto>>> Update(long id, [FromBody] UpdateIamRolePermissionDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/iam/user-role-assignments")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS IAM — user role assignments")]
public sealed class IamUserRoleAssignmentsController : ControllerBase
{
    private readonly IIamUserRoleAssignmentService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<IamUserRoleAssignmentsController> _logger;

    public IamUserRoleAssignmentsController(IIamUserRoleAssignmentService service, ITenantContext tenant, ILogger<IamUserRoleAssignmentsController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "IamUserRoleAssignments_GetById")]
    public async Task<ActionResult<BaseResponse<IamUserRoleAssignmentResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("IamUserRoleAssignment GetById {Id} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "IamUserRoleAssignments_GetPaged")]
    public async Task<ActionResult<BaseResponse<PagedResponse<IamUserRoleAssignmentResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<IamUserRoleAssignmentResponseDto>>> Create([FromBody] CreateIamUserRoleAssignmentDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<IamUserRoleAssignmentResponseDto>>> Update(long id, [FromBody] UpdateIamUserRoleAssignmentDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/iam/user-facility-scopes")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS IAM — user facility scopes")]
public sealed class IamUserFacilityScopesController : ControllerBase
{
    private readonly IIamUserFacilityScopeService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<IamUserFacilityScopesController> _logger;

    public IamUserFacilityScopesController(IIamUserFacilityScopeService service, ITenantContext tenant, ILogger<IamUserFacilityScopesController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "IamUserFacilityScopes_GetById")]
    public async Task<ActionResult<BaseResponse<IamUserFacilityScopeResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("IamUserFacilityScope GetById {Id} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "IamUserFacilityScopes_GetPaged")]
    public async Task<ActionResult<BaseResponse<PagedResponse<IamUserFacilityScopeResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<IamUserFacilityScopeResponseDto>>> Create([FromBody] CreateIamUserFacilityScopeDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<IamUserFacilityScopeResponseDto>>> Update(long id, [FromBody] UpdateIamUserFacilityScopeDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/iam/user-mfa-factors")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS IAM — user MFA factors")]
public sealed class IamUserMfaFactorsController : ControllerBase
{
    private readonly IIamUserMfaFactorService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<IamUserMfaFactorsController> _logger;

    public IamUserMfaFactorsController(IIamUserMfaFactorService service, ITenantContext tenant, ILogger<IamUserMfaFactorsController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "IamUserMfaFactors_GetById")]
    public async Task<ActionResult<BaseResponse<IamUserMfaFactorResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("IamUserMfaFactor GetById {Id} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "IamUserMfaFactors_GetPaged")]
    public async Task<ActionResult<BaseResponse<PagedResponse<IamUserMfaFactorResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<IamUserMfaFactorResponseDto>>> Create([FromBody] CreateIamUserMfaFactorDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<IamUserMfaFactorResponseDto>>> Update(long id, [FromBody] UpdateIamUserMfaFactorDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/iam/password-reset-tokens")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS IAM — password reset tokens")]
public sealed class IamPasswordResetTokensController : ControllerBase
{
    private readonly IIamPasswordResetTokenService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<IamPasswordResetTokensController> _logger;

    public IamPasswordResetTokensController(IIamPasswordResetTokenService service, ITenantContext tenant, ILogger<IamPasswordResetTokensController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "IamPasswordResetTokens_GetById")]
    public async Task<ActionResult<BaseResponse<IamPasswordResetTokenResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("IamPasswordResetToken GetById {Id} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "IamPasswordResetTokens_GetPaged")]
    public async Task<ActionResult<BaseResponse<PagedResponse<IamPasswordResetTokenResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<IamPasswordResetTokenResponseDto>>> Create([FromBody] CreateIamPasswordResetTokenDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<IamPasswordResetTokenResponseDto>>> Update(long id, [FromBody] UpdateIamPasswordResetTokenDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/iam/user-session-activities")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS IAM — user session activities")]
public sealed class IamUserSessionActivitiesController : ControllerBase
{
    private readonly IIamUserSessionActivityService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<IamUserSessionActivitiesController> _logger;

    public IamUserSessionActivitiesController(IIamUserSessionActivityService service, ITenantContext tenant, ILogger<IamUserSessionActivitiesController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "IamUserSessionActivities_GetById")]
    public async Task<ActionResult<BaseResponse<IamUserSessionActivityResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("IamUserSessionActivity GetById {Id} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "IamUserSessionActivities_GetPaged")]
    public async Task<ActionResult<BaseResponse<PagedResponse<IamUserSessionActivityResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<IamUserSessionActivityResponseDto>>> Create([FromBody] CreateIamUserSessionActivityDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<IamUserSessionActivityResponseDto>>> Update(long id, [FromBody] UpdateIamUserSessionActivityDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

#endregion

#region Catalog / billing lines (facility-scoped services)

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/test-package-lines")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS — test package lines")]
public sealed class TestPackageLinesController : ControllerBase
{
    private readonly ILmsTestPackageLineService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<TestPackageLinesController> _logger;

    public TestPackageLinesController(ILmsTestPackageLineService service, ITenantContext tenant, ILogger<TestPackageLinesController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "TestPackageLines_GetById")]
    public async Task<ActionResult<BaseResponse<TestPackageLineResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("TestPackageLine GetById {Id} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "TestPackageLines_GetPaged")]
    public async Task<ActionResult<BaseResponse<PagedResponse<TestPackageLineResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<TestPackageLineResponseDto>>> Create([FromBody] CreateTestPackageLineDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<TestPackageLineResponseDto>>> Update(long id, [FromBody] UpdateTestPackageLineDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/test-prices")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS — test prices")]
public sealed class TestPricesController : ControllerBase
{
    private readonly ILmsTestPriceService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<TestPricesController> _logger;

    public TestPricesController(ILmsTestPriceService service, ITenantContext tenant, ILogger<TestPricesController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "TestPrices_GetById")]
    public async Task<ActionResult<BaseResponse<TestPriceResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("TestPrice GetById {Id} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "TestPrices_GetPaged")]
    public async Task<ActionResult<BaseResponse<PagedResponse<TestPriceResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<TestPriceResponseDto>>> Create([FromBody] CreateTestPriceDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<TestPriceResponseDto>>> Update(long id, [FromBody] UpdateTestPriceDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/lab-invoice-lines")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS — lab invoice lines")]
public sealed class LabInvoiceLinesController : ControllerBase
{
    private readonly ILmsLabInvoiceLineService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<LabInvoiceLinesController> _logger;

    public LabInvoiceLinesController(ILmsLabInvoiceLineService service, ITenantContext tenant, ILogger<LabInvoiceLinesController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "LabInvoiceLines_GetById")]
    public async Task<ActionResult<BaseResponse<LabInvoiceLineResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("LabInvoiceLine GetById {Id} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "LabInvoiceLines_GetPaged")]
    public async Task<ActionResult<BaseResponse<PagedResponse<LabInvoiceLineResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<LabInvoiceLineResponseDto>>> Create([FromBody] CreateLabInvoiceLineDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<LabInvoiceLineResponseDto>>> Update(long id, [FromBody] UpdateLabInvoiceLineDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/lab-payment-transactions")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS — lab payment transactions")]
public sealed class LabPaymentTransactionsController : ControllerBase
{
    private readonly ILmsLabPaymentTransactionService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<LabPaymentTransactionsController> _logger;

    public LabPaymentTransactionsController(ILmsLabPaymentTransactionService service, ITenantContext tenant, ILogger<LabPaymentTransactionsController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "LabPaymentTransactions_GetById")]
    public async Task<ActionResult<BaseResponse<LabPaymentTransactionResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("LabPaymentTransaction GetById {Id} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "LabPaymentTransactions_GetPaged")]
    public async Task<ActionResult<BaseResponse<PagedResponse<LabPaymentTransactionResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<LabPaymentTransactionResponseDto>>> Create([FromBody] CreateLabPaymentTransactionDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<LabPaymentTransactionResponseDto>>> Update(long id, [FromBody] UpdateLabPaymentTransactionDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/patient-wallet-accounts")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS — patient wallet accounts")]
public sealed class PatientWalletAccountsController : ControllerBase
{
    private readonly ILmsPatientWalletAccountService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<PatientWalletAccountsController> _logger;

    public PatientWalletAccountsController(ILmsPatientWalletAccountService service, ITenantContext tenant, ILogger<PatientWalletAccountsController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "PatientWalletAccounts_GetById")]
    public async Task<ActionResult<BaseResponse<PatientWalletAccountResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("PatientWalletAccount GetById {Id} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "PatientWalletAccounts_GetPaged")]
    public async Task<ActionResult<BaseResponse<PagedResponse<PatientWalletAccountResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<PatientWalletAccountResponseDto>>> Create([FromBody] CreatePatientWalletAccountDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<PatientWalletAccountResponseDto>>> Update(long id, [FromBody] UpdatePatientWalletAccountDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/patient-wallet-transactions")]
[RequirePermission(TriVitaPermissions.LmsApi)]
[SwaggerTag("LMS — patient wallet transactions")]
public sealed class PatientWalletTransactionsController : ControllerBase
{
    private readonly ILmsPatientWalletTransactionService _service;
    private readonly ITenantContext _tenant;
    private readonly ILogger<PatientWalletTransactionsController> _logger;

    public PatientWalletTransactionsController(ILmsPatientWalletTransactionService service, ITenantContext tenant, ILogger<PatientWalletTransactionsController> logger)
    {
        _service = service;
        _tenant = tenant;
        _logger = logger;
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(OperationId = "PatientWalletTransactions_GetById")]
    public async Task<ActionResult<BaseResponse<PatientWalletTransactionResponseDto>>> GetById(long id, CancellationToken ct)
    {
        _logger.LogInformation("PatientWalletTransaction GetById {Id} tenant {TenantId}", id, _tenant.TenantId);
        return Ok(await _service.GetByIdAsync(id, ct));
    }

    [HttpGet]
    [SwaggerOperation(OperationId = "PatientWalletTransactions_GetPaged")]
    public async Task<ActionResult<BaseResponse<PagedResponse<PatientWalletTransactionResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)
        => Ok(await _service.GetPagedAsync(query, ct));

    [HttpPost]
    public async Task<ActionResult<BaseResponse<PatientWalletTransactionResponseDto>>> Create([FromBody] CreatePatientWalletTransactionDto dto, CancellationToken ct)
        => Ok(await _service.CreateAsync(dto, ct));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<BaseResponse<PatientWalletTransactionResponseDto>>> Update(long id, [FromBody] UpdatePatientWalletTransactionDto dto, CancellationToken ct)
        => Ok(await _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)
        => Ok(await _service.DeleteAsync(id, ct));
}

#endregion
