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
[Route("api/v{version:apiVersion}/departments")]
[Authorize]
[RequirePermission(TriVitaPermissions.SharedApi)]
[SwaggerTag("Enterprise hierarchy")]
public sealed class DepartmentsController : ControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentsController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "List departments (optional facility filter)", OperationId = "Shared_Departments_List")]
    [ProducesResponseType(typeof(BaseResponse<IReadOnlyList<DepartmentResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<IReadOnlyList<DepartmentResponseDto>>>> List(
        [FromQuery] long? facilityId,
        CancellationToken cancellationToken) =>
        Ok(await _departmentService.ListAsync(facilityId, cancellationToken));

    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Get department by id", OperationId = "Shared_Departments_GetById")]
    [ProducesResponseType(typeof(BaseResponse<DepartmentResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<DepartmentResponseDto>>> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await _departmentService.GetByIdAsync(id, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create department", OperationId = "Shared_Departments_Create")]
    [ProducesResponseType(typeof(BaseResponse<DepartmentResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<DepartmentResponseDto>>> Create(
        [FromBody] CreateDepartmentDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _departmentService.CreateAsync(dto, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id:long}")]
    [SwaggerOperation(Summary = "Update department", OperationId = "Shared_Departments_Update")]
    [ProducesResponseType(typeof(BaseResponse<DepartmentResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<DepartmentResponseDto>>> Update(
        long id,
        [FromBody] UpdateDepartmentDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _departmentService.UpdateAsync(id, dto, cancellationToken);
        if (!result.Success && result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
            return NotFound(result);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id:long}")]
    [SwaggerOperation(Summary = "Soft-delete department", OperationId = "Shared_Departments_Delete")]
    [ProducesResponseType(typeof(BaseResponse<object?>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await _departmentService.DeleteAsync(id, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
