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
[Route("api/v{version:apiVersion}/companies")]
[Authorize]
[RequirePermission(TriVitaPermissions.SharedApi)]
[SwaggerTag("Enterprise hierarchy")]
public sealed class CompaniesController : ControllerBase
{
    private readonly ICompanyService _companyService;

    public CompaniesController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "List companies (optional enterprise filter)", OperationId = "Shared_Companies_List")]
    [ProducesResponseType(typeof(BaseResponse<IReadOnlyList<CompanyResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<IReadOnlyList<CompanyResponseDto>>>> List(
        [FromQuery] long? enterpriseId,
        CancellationToken cancellationToken) =>
        Ok(await _companyService.ListAsync(enterpriseId, cancellationToken));

    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Get company by id", OperationId = "Shared_Companies_GetById")]
    [ProducesResponseType(typeof(BaseResponse<CompanyResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<CompanyResponseDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await _companyService.GetByIdAsync(id, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create company", OperationId = "Shared_Companies_Create")]
    [ProducesResponseType(typeof(BaseResponse<CompanyResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<CompanyResponseDto>>> Create(
        [FromBody] CreateCompanyDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _companyService.CreateAsync(dto, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:long}")]
    [SwaggerOperation(Summary = "Update company", OperationId = "Shared_Companies_Update")]
    [ProducesResponseType(typeof(BaseResponse<CompanyResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<CompanyResponseDto>>> Update(
        long id,
        [FromBody] UpdateCompanyDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _companyService.UpdateAsync(id, dto, cancellationToken);
        if (!result.Success && result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
            return NotFound(result);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:long}")]
    [SwaggerOperation(Summary = "Soft-delete company", OperationId = "Shared_Companies_Delete")]
    [ProducesResponseType(typeof(BaseResponse<object?>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await _companyService.DeleteAsync(id, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
