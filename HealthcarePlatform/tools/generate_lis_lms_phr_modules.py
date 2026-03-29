"""
Generates Domain entities, EF configurations, DTOs, services, controllers, and mapping snippets
for LIS / LMS / Pharmacy from 03_LIS.sql, 04_LMS.sql, 05_Pharmacy.sql.

Run from repo: python tools/generate_lis_lms_phr_modules.py
"""
from __future__ import annotations

import re
from pathlib import Path
from dataclasses import dataclass, field
from typing import Any

ROOT = Path(__file__).resolve().parents[1]

BASE_COLS = {
    "Id", "TenantId", "FacilityId", "IsActive", "IsDeleted",
    "CreatedOn", "CreatedBy", "ModifiedOn", "ModifiedBy", "RowVersion",
}

# Explicit class names (singular, prefixed)
CLASS_NAMES: dict[tuple[str, str], str] = {
    ("LIS", "LIS_TestCategory"): "LisTestCategory",
    ("LIS", "LIS_SampleType"): "LisSampleType",
    ("LIS", "LIS_TestMaster"): "LisTestMaster",
    ("LIS", "LIS_TestParameters"): "LisTestParameter",
    ("LIS", "LIS_TestReferenceRanges"): "LisTestReferenceRange",
    ("LIS", "LIS_LabOrder"): "LisLabOrder",
    ("LIS", "LIS_LabOrderItems"): "LisLabOrderItem",
    ("LIS", "LIS_OrderStatusHistory"): "LisOrderStatusHistory",
    ("LIS", "LIS_SampleCollection"): "LisSampleCollection",
    ("LIS", "LIS_SampleTracking"): "LisSampleTracking",
    ("LIS", "LIS_LabResults"): "LisLabResult",
    ("LIS", "LIS_ResultApproval"): "LisResultApproval",
    ("LIS", "LIS_ResultHistory"): "LisResultHistory",
    ("LIS", "LIS_ReportHeader"): "LisReportHeader",
    ("LIS", "LIS_ReportDetails"): "LisReportDetail",
    ("LMS", "ProcessingStages"): "LmsProcessingStage",
    ("LMS", "WorkQueue"): "LmsWorkQueue",
    ("LMS", "TechnicianAssignment"): "LmsTechnicianAssignment",
    ("LMS", "Equipment"): "LmsEquipment",
    ("LMS", "EquipmentMaintenance"): "LmsEquipmentMaintenance",
    ("LMS", "EquipmentCalibration"): "LmsEquipmentCalibration",
    ("LMS", "QCRecords"): "LmsQcRecord",
    ("LMS", "QCResults"): "LmsQcResult",
    ("LMS", "LabInventory"): "LmsLabInventory",
    ("LMS", "LabInventoryTransactions"): "LmsLabInventoryTransaction",
    ("PHR", "MedicineCategory"): "PhrMedicineCategory",
    ("PHR", "Manufacturer"): "PhrManufacturer",
    ("PHR", "Composition"): "PhrComposition",
    ("PHR", "Medicine"): "PhrMedicine",
    ("PHR", "MedicineComposition"): "PhrMedicineComposition",
    ("PHR", "MedicineBatch"): "PhrMedicineBatch",
    ("PHR", "BatchStock"): "PhrBatchStock",
    ("PHR", "StockLedger"): "PhrStockLedger",
    ("PHR", "PurchaseOrder"): "PhrPurchaseOrder",
    ("PHR", "PurchaseOrderItems"): "PhrPurchaseOrderItem",
    ("PHR", "GoodsReceipt"): "PhrGoodsReceipt",
    ("PHR", "GoodsReceiptItems"): "PhrGoodsReceiptItem",
    ("PHR", "PharmacySales"): "PhrPharmacySale",
    ("PHR", "PharmacySalesItems"): "PhrPharmacySalesItem",
    ("PHR", "PrescriptionMapping"): "PhrPrescriptionMapping",
    ("PHR", "StockAdjustment"): "PhrStockAdjustment",
    ("PHR", "StockAdjustmentItems"): "PhrStockAdjustmentItem",
    ("PHR", "StockTransfer"): "PhrStockTransfer",
    ("PHR", "StockTransferItems"): "PhrStockTransferItem",
    ("PHR", "ExpiryTracking"): "PhrExpiryTracking",
}

MODULE_ROOT = {
    "LIS": ROOT / "LISService",
    "LMS": ROOT / "LMSService",
    "PHR": ROOT / "PharmacyService",
}

# Tenant-level catalogs: FacilityId nullable in SQL — do not require X-Facility-Id on create.
OPTIONAL_FACILITY_CLASS_NAMES: dict[str, set[str]] = {
    "LIS": {
        "LisTestCategory",
        "LisSampleType",
        "LisTestMaster",
        "LisTestParameter",
        "LisTestReferenceRange",
    },
    "LMS": set(),
    "PHR": {
        "PhrMedicineCategory",
        "PhrManufacturer",
        "PhrComposition",
        "PhrMedicine",
        "PhrMedicineComposition",
        "PhrMedicineBatch",
    },
}


@dataclass
class Col:
    name: str
    cs_type: str
    sql_type: str
    max_len: int | None = None


def pascal(s: str) -> str:
    return "".join(w.capitalize() for w in re.split(r"[_\s]+", s) if w)


def to_kebab(class_name: str, prefix: str) -> str:
    """LisTestCategory + Lis -> test-category"""
    if class_name.startswith(prefix):
        rest = class_name[len(prefix) :]
    else:
        rest = class_name
    s = re.sub(r"([a-z0-9])([A-Z])", r"\1-\2", rest)
    return s.lower()


def parse_column_line(line: str) -> Col | None:
    line = line.strip().rstrip(",")
    if not line or line.startswith("--"):
        return None
    ul = line.upper()
    if ul.startswith("CONSTRAINT") or ul.startswith("PRIMARY") or ul.startswith("FOREIGN"):
        return None
    # Strip CONSTRAINT / DEFAULT tails from same line
    for kw in (" CONSTRAINT ", " DEFAULT "):
        if kw in line.upper():
            line = line[: line.upper().index(kw)].strip().rstrip(",")
            break
    m = re.match(
        r"^(\w+)\s+(NVARCHAR\s*\([^)]+\)|BIGINT|INT|BIT|DECIMAL\s*\([^)]+\)|DATETIME2\s*\([^)]+\)|DATE|ROWVERSION)\s+(NOT\s+NULL|NULL)\s*$",
        line.strip().rstrip(","),
        re.I,
    )
    if not m:
        return None
    name, typ_raw, nullability = m.group(1), m.group(2).upper(), m.group(3).upper()
    nullable = nullability == "NULL"
    typ = typ_raw.split()[0].upper() if typ_raw else ""

    max_len = None
    if typ.startswith("NVARCHAR"):
        mx = re.search(r"NVARCHAR\((\d+|MAX)\)", typ_raw, re.I)
        if mx and mx.group(1).upper() != "MAX":
            max_len = int(mx.group(1))
        cs = "string?" if nullable else "string"
    elif typ == "BIGINT":
        cs = "long?" if nullable else "long"
    elif typ == "INT":
        cs = "int?" if nullable else "int"
    elif typ.startswith("DECIMAL"):
        cs = "decimal?" if nullable else "decimal"
    elif typ.startswith("DATETIME2"):
        cs = "DateTime?" if nullable else "DateTime"
    elif typ == "DATE":
        cs = "DateTime?" if nullable else "DateTime"
    elif typ == "BIT":
        cs = "bool?" if nullable else "bool"
    elif typ == "ROWVERSION":
        return None
    else:
        cs = "string?"

    return Col(name=name, cs_type=cs, sql_type=typ_raw.upper(), max_len=max_len)


def extract_table_block(sql: str, table: str) -> str | None:
    pat = rf"CREATE TABLE\s+dbo\.{re.escape(table)}\s*\("
    m = re.search(pat, sql, re.I | re.S)
    if not m:
        return None
    start = m.end() - 1
    depth = 0
    for i in range(start, len(sql)):
        if sql[i] == "(":
            depth += 1
        elif sql[i] == ")":
            depth -= 1
            if depth == 0:
                return sql[start : i + 1]
    return None


def parse_table_columns(inner: str) -> list[Col]:
    cols: list[Col] = []
    for line in inner.splitlines():
        ls = line.strip()
        if not ls:
            continue
        u = ls.upper()
        if u.startswith("CONSTRAINT") or u.startswith("PRIMARY") or u.startswith("FOREIGN"):
            break
        c = parse_column_line(ls)
        if c and c.name not in BASE_COLS:
            cols.append(c)
    return cols


def emit_entity(ns: str, class_name: str, table: str, columns: list[Col]) -> str:
    lines = [
        "using Healthcare.Common.Entities;",
        "",
        f"namespace {ns};",
        "",
        f"public sealed class {class_name} : BaseEntity",
        "{",
    ]
    for c in columns:
        init = ""
        if c.cs_type == "string":
            init = " = null!;"
        elif c.cs_type == "bool":
            init = ""
        lines.append(f"    public {c.cs_type} {c.name} {{ get; set; }}{init}")
    lines.append("}")
    return "\n".join(lines)


def emit_config(domain_ns: str, cfg_ns: str, class_name: str, table: str, columns: list[Col]) -> str:
    lines = [
        f"using {domain_ns};",
        "using Microsoft.EntityFrameworkCore;",
        "using Microsoft.EntityFrameworkCore.Metadata.Builders;",
        "",
        f"namespace {cfg_ns};",
        "",
        f"public sealed class {class_name}Configuration : IEntityTypeConfiguration<{class_name}>",
        "{",
        f"    public void Configure(EntityTypeBuilder<{class_name}> builder)",
        "    {",
        f'        builder.ToTable("{table}");',
        "        builder.HasKey(e => e.Id);",
        "        builder.Property(e => e.RowVersion).IsRowVersion();",
    ]
    for c in columns:
        if c.name in BASE_COLS:
            continue
        if c.max_len:
            lines.append(f"        builder.Property(e => e.{c.name}).HasMaxLength({c.max_len});")
        elif "NVARCHAR(MAX)" in c.sql_type.upper():
            lines.append(f'        builder.Property(e => e.{c.name}).HasColumnType("nvarchar(max)");')
        elif "DECIMAL" in c.sql_type.upper():
            lines.append(f"        builder.Property(e => e.{c.name}).HasPrecision(18, 4);")
        elif c.sql_type.upper().startswith("DATE") and "DATETIME" not in c.sql_type.upper():
            lines.append(f'        builder.Property(e => e.{c.name}).HasColumnType("date");')
    lines.extend(["    }", "}"])
    return "\n".join(lines)


def emit_dtos(dto_ns: str, short: str, columns: list[Col]) -> tuple[str, str, str]:
    create_f = []
    update_f = []
    resp_f = ["    public long Id { get; set; }", "    public long TenantId { get; set; }", "    public long? FacilityId { get; set; }"]
    for c in columns:
        create_f.append(f"    public {c.cs_type} {c.name} {{ get; set; }}")
        update_f.append(f"    public {c.cs_type} {c.name} {{ get; set; }}")
        resp_f.append(f"    public {c.cs_type} {c.name} {{ get; set; }}")
    create = "\n".join([f"namespace {dto_ns};", "", f"public sealed class Create{short}Dto", "{", *create_f, "}"])
    update = "\n".join([f"namespace {dto_ns};", "", f"public sealed class Update{short}Dto", "{", *update_f, "}"])
    resp = "\n".join([f"namespace {dto_ns};", "", f"public sealed class {short}ResponseDto", "{", *resp_f, "}"])
    return create, update, resp


def map_profile_lines(module: str, profile_class: str, class_name: str, short: str, ns_entity: str, ns_dto: str) -> list[str]:
    ign = """.ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore())"""
    return [
        f"        CreateMap<{class_name}, {short}ResponseDto>();",
        f"        CreateMap<Create{short}Dto, {class_name}>()",
        f"            {ign};",
        f"        CreateMap<Update{short}Dto, {class_name}>()",
        f"            {ign};",
        "",
    ]


def process_sql_file(module: str, sql_path: Path, root_assembly: str, profile_lines: list[str]) -> None:
    sql = sql_path.read_text(encoding="utf-8", errors="replace")
    root = MODULE_ROOT[module]
    domain_ns = f"{root_assembly}.Domain.Entities"
    cfg_ns = f"{root_assembly}.Infrastructure.Persistence.Configurations"
    dto_ns = f"{root_assembly}.Application.DTOs.Entities"
    svc_ns = f"{root_assembly}.Application.Services.Entities"
    api_ns = f"{root_assembly}.API.Controllers.v1.Entities"
    ent_dir = root / f"{root_assembly}.Domain" / "Entities"
    cfg_dir = root / f"{root_assembly}.Infrastructure" / "Persistence" / "Configurations"
    dto_dir = root / f"{root_assembly}.Application" / "DTOs" / "Entities"
    svc_dir = root / f"{root_assembly}.Application" / "Services" / "Entities"
    ctrl_dir = root / f"{root_assembly}.API" / "Controllers" / "v1" / "Entities"
    pfx = {"LIS": "Lis", "LMS": "Lms", "PHR": "Phr"}[module]
    for d in (ent_dir, cfg_dir, dto_dir, svc_dir, ctrl_dir):
        d.mkdir(parents=True, exist_ok=True)

    for (mod, table), class_name in CLASS_NAMES.items():
        if mod != module:
            continue
        inner = extract_table_block(sql, table)
        if not inner:
            print("MISSING", table)
            continue
        cols = parse_table_columns(inner)
        short = class_name
        if short.startswith("Lis"):
            short = short[3:]
        elif short.startswith("Lms"):
            short = short[3:]
        elif short.startswith("Phr"):
            short = short[3:]

        (ent_dir / f"{class_name}.cs").write_text(emit_entity(domain_ns, class_name, table, cols), encoding="utf-8")
        (cfg_dir / f"{class_name}Configuration.cs").write_text(
            emit_config(domain_ns, cfg_ns, class_name, table, cols), encoding="utf-8"
        )

        c, u, r = emit_dtos(dto_ns, short, cols)
        (dto_dir / f"Create{short}Dto.cs").write_text(c, encoding="utf-8")
        (dto_dir / f"Update{short}Dto.cs").write_text(u, encoding="utf-8")
        (dto_dir / f"{short}ResponseDto.cs").write_text(r, encoding="utf-8")

        profile_lines.extend(map_profile_lines(module, "", class_name, short, domain_ns, dto_ns))

        route = to_kebab(class_name, {"LIS": "Lis", "LMS": "Lms", "PHR": "Phr"}[module])
        ctrl = emit_controller(api_ns, svc_ns, dto_ns, class_name, short, route, module)
        (ctrl_dir / f"{short}Controller.cs").write_text(ctrl, encoding="utf-8")

        svc = emit_service(svc_ns, domain_ns, dto_ns, root_assembly, class_name, short, module)
        (svc_dir / f"{pfx}{short}Service.cs").write_text(svc, encoding="utf-8")

    print(f"Done {module}")


def emit_controller(api_ns, svc_ns, dto_ns, class_name: str, short: str, route: str, module: str) -> str:
    pfx = {"LIS": "Lis", "LMS": "Lms", "PHR": "Phr"}[module]
    iface = f"I{pfx}{short}Service"
    return "\n".join([
        "using Asp.Versioning;",
        "using Healthcare.Common.MultiTenancy;",
        "using Healthcare.Common.Pagination;",
        "using Healthcare.Common.Responses;",
        f"using {dto_ns};",
        f"using {svc_ns};",
        "using Microsoft.AspNetCore.Authorization;",
        "using Microsoft.AspNetCore.Mvc;",
        "using Swashbuckle.AspNetCore.Annotations;",
        "",
        f"namespace {api_ns};",
        "",
        "[ApiController]",
        '[ApiVersion("1.0")]',
        f'[Route("api/v{{version:apiVersion}}/{route}")]',
        "[Authorize]",
        f'[SwaggerTag("{class_name}")]',
        f"public sealed class {short}Controller : ControllerBase",
        "{",
        f"    private readonly {iface} _service;",
        "    private readonly ITenantContext _tenant;",
        f"    private readonly ILogger<{short}Controller> _logger;",
        "",
        f"    public {short}Controller({iface} service, ITenantContext tenant, ILogger<{short}Controller> logger)",
        "    { _service = service; _tenant = tenant; _logger = logger; }",
        "",
        '    [HttpGet("{id:long}")]',
        f'    [SwaggerOperation(OperationId = "{short}_GetById")]',
        f'    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<{short}ResponseDto>))]',
        f"    public async Task<ActionResult<BaseResponse<{short}ResponseDto>>> GetById(long id, CancellationToken ct)",
        "    {",
        '        _logger.LogInformation("GetById {EntityId} tenant {TenantId}", id, _tenant.TenantId);',
        f"        return Ok(await _service.GetByIdAsync(id, ct));",
        "    }",
        "",
        "    [HttpGet]",
        f'    [SwaggerOperation(OperationId = "{short}_GetPaged")]',
        f'    [SwaggerResponse(StatusCodes.Status200OK, "OK", typeof(BaseResponse<PagedResponse<{short}ResponseDto>>))]',
        f"    public async Task<ActionResult<BaseResponse<PagedResponse<{short}ResponseDto>>>> GetPaged([FromQuery] PagedQuery query, CancellationToken ct)",
        f"        => Ok(await _service.GetPagedAsync(query, ct));",
        "",
        "    [HttpPost]",
        f"    public async Task<ActionResult<BaseResponse<{short}ResponseDto>>> Create([FromBody] Create{short}Dto dto, CancellationToken ct)",
        f"        => Ok(await _service.CreateAsync(dto, ct));",
        "",
        '    [HttpPut("{id:long}")]',
        f"    public async Task<ActionResult<BaseResponse<{short}ResponseDto>>> Update(long id, [FromBody] Update{short}Dto dto, CancellationToken ct)",
        f"        => Ok(await _service.UpdateAsync(id, dto, ct));",
        "",
        '    [HttpDelete("{id:long}")]',
        f"    public async Task<ActionResult<BaseResponse<object?>>> Delete(long id, CancellationToken ct)",
        f"        => Ok(await _service.DeleteAsync(id, ct));",
        "}",
        "",
    ])


def emit_service(svc_ns, domain_ns, dto_ns, root_assembly: str, class_name: str, short: str, module: str) -> str:
    pfx = {"LIS": "Lis", "LMS": "Lms", "PHR": "Phr"}[module]
    base = f"{pfx}CrudServiceBase"
    iface = f"I{pfx}{short}Service"
    repo_ns = f"{root_assembly}.Domain.Repositories"
    ext_ns = f"{root_assembly}.Application.Services.Extended"
    req_fac = str(class_name not in OPTIONAL_FACILITY_CLASS_NAMES[module]).lower()
    return "\n".join([
        "using AutoMapper;",
        "using FluentValidation;",
        "using Healthcare.Common.Pagination;",
        "using Healthcare.Common.Responses;",
        f"using {domain_ns};",
        f"using {dto_ns};",
        f"using {repo_ns};",
        f"using {ext_ns};",
        "using Microsoft.Extensions.Logging;",
        "",
        f"namespace {svc_ns};",
        "",
        f"public interface {iface}",
        "{",
        f"    Task<BaseResponse<{short}ResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);",
        f"    Task<BaseResponse<PagedResponse<{short}ResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);",
        f"    Task<BaseResponse<{short}ResponseDto>> CreateAsync(Create{short}Dto dto, CancellationToken cancellationToken = default);",
        f"    Task<BaseResponse<{short}ResponseDto>> UpdateAsync(long id, Update{short}Dto dto, CancellationToken cancellationToken = default);",
        "    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);",
        "}",
        "",
        f"public sealed class {pfx}{short}Service : {base}<{class_name}, Create{short}Dto, Update{short}Dto, {short}ResponseDto, {pfx}{short}Service>, {iface}",
        "{",
        f"    public {pfx}{short}Service(",
        f"        IRepository<{class_name}> repository,",
        "        IMapper mapper,",
        "        Healthcare.Common.MultiTenancy.ITenantContext tenant,",
        f"        IValidator<Create{short}Dto>? createValidator,",
        f"        IValidator<Update{short}Dto>? updateValidator,",
        f"        ILogger<{pfx}{short}Service> logger)",
        f"        : base(repository, mapper, tenant, createValidator, updateValidator, logger) {{ }}",
        "",
        f"    protected override bool RequiresFacilityId => {req_fac};",
        "",
        f"    public Task<BaseResponse<PagedResponse<{short}ResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)",
        "        => GetPagedCoreAsync(query, null, cancellationToken);",
        "}",
        "",
    ])


def main() -> None:
    profiles: dict[str, list[str]] = {"LIS": [], "LMS": [], "PHR": []}
    process_sql_file("LIS", ROOT.parent / "03_LIS.sql", "LISService", profiles["LIS"])
    process_sql_file("LMS", ROOT.parent / "04_LMS.sql", "LMSService", profiles["LMS"])
    process_sql_file("PHR", ROOT.parent / "05_Pharmacy.sql", "PharmacyService", profiles["PHR"])

    asm = {"LIS": "LISService", "LMS": "LMSService", "PHR": "PharmacyService"}
    prof_short = {"LIS": "Lis", "LMS": "Lms", "PHR": "Phr"}
    for mod, lines in profiles.items():
        ra = asm[mod]
        prof_path = MODULE_ROOT[mod] / f"{ra}.Application" / "Mapping" / f"{prof_short[mod]}GeneratedMappingProfile.cs"
        prof_path.parent.mkdir(parents=True, exist_ok=True)
        pnm = f"{prof_short[mod]}GeneratedMappingProfile"
        body = "\n".join([
            "using AutoMapper;",
            f"using {ra}.Application.DTOs.Entities;",
            f"using {ra}.Domain.Entities;",
            f"namespace {ra}.Application.Mapping;",
            "",
            f"public sealed class {pnm} : Profile",
            "{",
            f"    public {pnm}()",
            "    {",
            *lines,
            "    }",
            "}",
        ])
        prof_path.write_text(body, encoding="utf-8")
        print("Wrote", prof_path)


if __name__ == "__main__":
    main()
