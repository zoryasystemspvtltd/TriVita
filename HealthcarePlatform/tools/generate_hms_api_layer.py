"""Generates HmsExtendedMappingProfile, validators, controllers for HMS extended entities."""
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
MAPPING = ROOT / "HMSService" / "HMSService.Application" / "Mapping" / "HmsExtendedMappingProfile.cs"
VAL_DIR = ROOT / "HMSService" / "HMSService.Application" / "Validation" / "Extended"
CTRL_DIR = ROOT / "HMSService" / "HMSService.API" / "Controllers" / "v1" / "Extended"

VAL_DIR.mkdir(parents=True, exist_ok=True)
CTRL_DIR.mkdir(parents=True, exist_ok=True)

IGNORE = """.ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore())"""

# (EntityClass, DtoBase, route_segment, controller_name, swagger_tag, filter_lines)
# filter_lines: list of (param_name, param_type, entity_prop) for GetPaged
SPECS = [
    ("HmsAppointmentStatusHistory", "AppointmentStatusHistory", "appointment-status-history", "AppointmentStatusHistory", "HMS Appointment status history",
     [("appointmentId", "long?", "AppointmentId")]),
    ("HmsAppointmentQueue", "AppointmentQueue", "appointment-queue", "AppointmentQueue", "HMS Appointment queue",
     [("appointmentId", "long?", "AppointmentId")]),
    ("HmsVital", "Vital", "vitals", "Vitals", "HMS Vitals",
     [("visitId", "long?", "VisitId")]),
    ("HmsClinicalNote", "ClinicalNote", "clinical-notes", "ClinicalNotes", "HMS Clinical notes",
     [("visitId", "long?", "VisitId")]),
    ("HmsDiagnosis", "Diagnosis", "diagnoses", "Diagnoses", "HMS Diagnoses",
     [("visitId", "long?", "VisitId")]),
    ("HmsMedicalProcedure", "MedicalProcedure", "procedures", "MedicalProcedures", "HMS Procedures",
     [("visitId", "long?", "VisitId")]),
    ("HmsPrescription", "Prescription", "prescriptions", "Prescriptions", "HMS Prescriptions",
     [("visitId", "long?", "VisitId"), ("patientId", "long?", "PatientId")]),
    ("HmsPrescriptionItem", "PrescriptionItem", "prescription-items", "PrescriptionItems", "HMS Prescription items",
     [("prescriptionId", "long?", "PrescriptionId")]),
    ("HmsPrescriptionNote", "PrescriptionNote", "prescription-notes", "PrescriptionNotes", "HMS Prescription notes",
     [("prescriptionId", "long?", "PrescriptionId")]),
    ("HmsPaymentMode", "PaymentMode", "payment-modes", "PaymentModes", "HMS Payment modes",
     []),
    ("HmsBillingHeader", "BillingHeader", "billing-headers", "BillingHeaders", "HMS Billing headers",
     [("visitId", "long?", "VisitId"), ("patientId", "long?", "PatientId")]),
    ("HmsBillingItem", "BillingItem", "billing-items", "BillingItems", "HMS Billing items",
     [("billingHeaderId", "long?", "BillingHeaderId")]),
    ("HmsPaymentTransaction", "PaymentTransaction", "payment-transactions", "PaymentTransactions", "HMS Payment transactions",
     [("billingHeaderId", "long?", "BillingHeaderId")]),
]


def emit_mapping():
    lines = [
        "using AutoMapper;",
        "using HMSService.Application.DTOs.Extended;",
        "using HMSService.Domain.Entities;",
        "",
        "namespace HMSService.Application.Mapping;",
        "",
        "/// <summary>AutoMapper profile for HMS extended (clinical/billing) entities.</summary>",
        "public sealed class HmsExtendedMappingProfile : Profile",
        "{",
        "    public HmsExtendedMappingProfile()",
        "    {",
    ]
    for ent, dto_base, _, _, _, _ in SPECS:
        lines.append(f"        CreateMap<{ent}, {dto_base}ResponseDto>();")
        lines.append(f"        CreateMap<Create{dto_base}Dto, {ent}>()")
        lines.append(f"            {IGNORE};")
        lines.append(f"        CreateMap<Update{dto_base}Dto, {ent}>()")
        lines.append(f"            {IGNORE};")
        lines.append("")
    lines.extend(["    }", "}", ""])
    return "\n".join(lines)


def emit_validator(dto_base, is_create: bool):
    kind = "Create" if is_create else "Update"
    cls = f"{kind}{dto_base}Dto"
    vname = f"{kind}{dto_base}Validator"
    return "\n".join([
        "using FluentValidation;",
        "using HMSService.Application.DTOs.Extended;",
        "",
        "namespace HMSService.Application.Validation.Extended;",
        "",
        f"public sealed class {vname} : AbstractValidator<{cls}>",
        "{",
        f"    public {vname}()",
        "    {",
        "        // Minimal rules; extend per business rules.",
        "    }",
        "}",
        "",
    ])


def emit_controller(ent, dto_base, route, ctrl_name, tag, filters):
    resp = f"{dto_base}ResponseDto"
    create = f"Create{dto_base}Dto"
    update = f"Update{dto_base}Dto"
    iface = f"I{dto_base}Service"

    filter_params = ", ".join([f"[FromQuery] {p} {name}" for name, p, _ in filters])
    filter_pass = ", ".join([name for name, _, _ in filters]) if filters else ""

    get_paged_sig = f"[FromQuery] PagedQuery query{', ' + filter_params if filter_params else ''}, CancellationToken cancellationToken"

    lines = [
        "using Asp.Versioning;",
        "using Healthcare.Common.MultiTenancy;",
        "using Healthcare.Common.Pagination;",
        "using Healthcare.Common.Responses;",
        f"using HMSService.Application.DTOs.Extended;",
        "using HMSService.Application.Services.Extended;",
        "using Microsoft.AspNetCore.Authorization;",
        "using Microsoft.AspNetCore.Mvc;",
        "using Swashbuckle.AspNetCore.Annotations;",
        "",
        "namespace HMSService.API.Controllers.v1.Extended;",
        "",
        "/// <summary>",
        f"/// REST API for {tag}.",
        "/// </summary>",
        "[ApiController]",
        '[ApiVersion("1.0")]',
        f'[Route("api/v{{version:apiVersion}}/{route}")]',
        "[Authorize]",
        f'[SwaggerTag("{tag}")]',
        f"public sealed class {ctrl_name}Controller : ControllerBase",
        "{",
        f"    private readonly {iface} _service;",
        "    private readonly ITenantContext _tenant;",
        f"    private readonly ILogger<{ctrl_name}Controller> _logger;",
        "",
        f"    public {ctrl_name}Controller({iface} service, ITenantContext tenant, ILogger<{ctrl_name}Controller> logger)",
        "    {",
        "        _service = service;",
        "        _tenant = tenant;",
        "        _logger = logger;",
        "    }",
        "",
        "    /// <summary>Gets an entity by id.</summary>",
        "    [HttpGet(\"{id:long}\")]",
        f'    [SwaggerOperation(Summary = "Get by id", OperationId = "{ctrl_name}_GetById")]',
        f'    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<{resp}>))]',
        f"    [ProducesResponseType(typeof(BaseResponse<{resp}>), StatusCodes.Status200OK)]",
        f"    public async Task<ActionResult<BaseResponse<{resp}>>> GetById(long id, CancellationToken cancellationToken)",
        "    {",
        f'        _logger.LogInformation("HMS {ctrl_name} GetById id {{Id}} tenant {{TenantId}}", id, _tenant.TenantId);',
        f"        var result = await _service.GetByIdAsync(id, cancellationToken);",
        f'        _logger.LogInformation("HMS {ctrl_name} GetById success {{Success}}", result.Success);',
        "        return Ok(result);",
        "    }",
        "",
        "    /// <summary>Paged list with optional filters.</summary>",
        "    [HttpGet]",
        f'    [SwaggerOperation(Summary = "List paged", OperationId = "{ctrl_name}_GetPaged")]',
        f'    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<{resp}>>))]',
        f"    [ProducesResponseType(typeof(BaseResponse<PagedResponse<{resp}>>), StatusCodes.Status200OK)]",
        f"    public async Task<ActionResult<BaseResponse<PagedResponse<{resp}>>>> GetPaged({get_paged_sig})",
        "    {",
        f'        _logger.LogInformation("HMS {ctrl_name} GetPaged tenant {{TenantId}}", _tenant.TenantId);',
        f"        var result = await _service.GetPagedAsync(query{', ' + filter_pass if filter_pass else ''}, cancellationToken);",
        "        return Ok(result);",
        "    }",
        "",
        "    /// <summary>Creates a new record.</summary>",
        "    [HttpPost]",
        f'    [SwaggerOperation(Summary = "Create", OperationId = "{ctrl_name}_Create")]',
        f'    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<{resp}>))]',
        f"    public async Task<ActionResult<BaseResponse<{resp}>>> Create([FromBody] {create} dto, CancellationToken cancellationToken)",
        "    {",
        f"        var result = await _service.CreateAsync(dto, cancellationToken);",
        "        return Ok(result);",
        "    }",
        "",
        "    /// <summary>Updates an existing record.</summary>",
        "    [HttpPut(\"{id:long}\")]",
        f'    [SwaggerOperation(Summary = "Update", OperationId = "{ctrl_name}_Update")]',
        f"    public async Task<ActionResult<BaseResponse<{resp}>>> Update(long id, [FromBody] {update} dto, CancellationToken cancellationToken)",
        "    {",
        f"        var result = await _service.UpdateAsync(id, dto, cancellationToken);",
        "        return Ok(result);",
        "    }",
        "",
        "    /// <summary>Soft-deletes a record.</summary>",
        "    [HttpDelete(\"{id:long}\")]",
        f'    [SwaggerOperation(Summary = "Delete (soft)", OperationId = "{ctrl_name}_Delete")]',
        "    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken cancellationToken)",
        "    {",
        "        var result = await _service.DeleteAsync(id, cancellationToken);",
        "        return Ok(result);",
        "    }",
        "}",
        "",
    ]
    return "\n".join(lines)


def main():
    MAPPING.write_text(emit_mapping(), encoding="utf-8")
    for ent, dto_base, route, ctrl_name, tag, filters in SPECS:
        (VAL_DIR / f"Create{dto_base}Validator.cs").write_text(emit_validator(dto_base, True), encoding="utf-8")
        (VAL_DIR / f"Update{dto_base}Validator.cs").write_text(emit_validator(dto_base, False), encoding="utf-8")
        (CTRL_DIR / f"{ctrl_name}Controller.cs").write_text(
            emit_controller(ent, dto_base, route, ctrl_name, tag, filters), encoding="utf-8")
    print("Wrote mapping profile, validators, controllers.")


if __name__ == "__main__":
    main()
