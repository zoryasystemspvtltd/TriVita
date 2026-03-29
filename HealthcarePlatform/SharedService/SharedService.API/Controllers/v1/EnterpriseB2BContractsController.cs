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

/// <summary>B2B contracts (corporate / insurance) scoped to an enterprise; additive API.</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/enterprise-b2b-contracts")]
[Authorize]
[RequirePermission(TriVitaPermissions.SharedApi)]
[SwaggerTag("Enterprise B2B contracts")]
public sealed class EnterpriseB2BContractsController : ControllerBase
{
    private readonly IEnterpriseB2BContractService _contracts;

    public EnterpriseB2BContractsController(IEnterpriseB2BContractService contracts)
    {
        _contracts = contracts;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "List B2B contracts for an enterprise", OperationId = "Shared_B2BContracts_List")]
    [ProducesResponseType(typeof(BaseResponse<IReadOnlyList<EnterpriseB2BContractResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<IReadOnlyList<EnterpriseB2BContractResponseDto>>>> List(
        [FromQuery] long enterpriseId,
        CancellationToken cancellationToken)
    {
        var result = await _contracts.ListByEnterpriseAsync(enterpriseId, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Get B2B contract by id", OperationId = "Shared_B2BContracts_GetById")]
    [ProducesResponseType(typeof(BaseResponse<EnterpriseB2BContractResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<EnterpriseB2BContractResponseDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await _contracts.GetByIdAsync(id, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create B2B contract", OperationId = "Shared_B2BContracts_Create")]
    [ProducesResponseType(typeof(BaseResponse<EnterpriseB2BContractResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<EnterpriseB2BContractResponseDto>>> Create(
        [FromBody] CreateEnterpriseB2BContractDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _contracts.CreateAsync(dto, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:long}")]
    [SwaggerOperation(Summary = "Update B2B contract", OperationId = "Shared_B2BContracts_Update")]
    [ProducesResponseType(typeof(BaseResponse<EnterpriseB2BContractResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<EnterpriseB2BContractResponseDto>>> Update(
        long id,
        [FromBody] UpdateEnterpriseB2BContractDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _contracts.UpdateAsync(id, dto, cancellationToken);
        if (!result.Success && result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
            return NotFound(result);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:long}")]
    [SwaggerOperation(Summary = "Soft-delete B2B contract", OperationId = "Shared_B2BContracts_Delete")]
    [ProducesResponseType(typeof(BaseResponse<object?>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await _contracts.DeleteAsync(id, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
