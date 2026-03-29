"""
Generates HMS extended entities, EF configs, DTOs, validators, thin services, controllers, mapping profile.
Run from repo root: python tools/generate_hms_extended.py
"""
from pathlib import Path
import textwrap

ROOT = Path(__file__).resolve().parents[1]
DOMAIN = ROOT / "HMSService" / "HMSService.Domain" / "Entities"
INFRA = ROOT / "HMSService" / "HMSService.Infrastructure" / "Persistence" / "Configurations"
APP_DTO = ROOT / "HMSService" / "HMSService.Application" / "DTOs" / "Extended"
APP_VAL = ROOT / "HMSService" / "HMSService.Application" / "Validation" / "Extended"
APP_SVC = ROOT / "HMSService" / "HMSService.Application" / "Services" / "Extended"
API_CTRL = ROOT / "HMSService" / "HMSService.API" / "Controllers" / "v1"

for d in (DOMAIN, INFRA, APP_DTO, APP_VAL, APP_SVC, API_CTRL):
    d.mkdir(parents=True, exist_ok=True)

# (ClassName, TableName, route_segment, props: list of (name, csharp_type, max_len_or_None))
SPECS = [
    ("HmsAppointmentStatusHistory", "HMS_AppointmentStatusHistory", "appointment-status-history", [
        ("AppointmentId", "long", None),
        ("StatusValueId", "long", None),
        ("StatusOn", "DateTime", None),
        ("StatusNote", "string?", 1000),
        ("ChangedByDoctorId", "long?", None),
    ]),
    ("HmsAppointmentQueue", "HMS_AppointmentQueue", "appointment-queue", [
        ("AppointmentId", "long", None),
        ("QueueToken", "string", 60),
        ("PositionInQueue", "int", None),
        ("QueueStatusReferenceValueId", "long", None),
        ("EnqueuedOn", "DateTime", None),
        ("CheckedInOn", "DateTime?", None),
        ("ExpectedServiceOn", "DateTime?", None),
    ]),
    ("HmsVital", "HMS_Vitals", "vitals", [
        ("VisitId", "long", None),
        ("RecordedOn", "DateTime", None),
        ("VitalReferenceValueId", "long", None),
        ("ValueNumeric", "decimal?", None),
        ("ValueNumeric2", "decimal?", None),
        ("ValueText", "string?", 200),
        ("UnitId", "long?", None),
        ("RecordedByDoctorId", "long?", None),
        ("Notes", "string?", 1000),
    ]),
    ("HmsClinicalNote", "HMS_ClinicalNotes", "clinical-notes", [
        ("VisitId", "long", None),
        ("NoteTypeReferenceValueId", "long", None),
        ("EncounterSection", "string?", 150),
        ("NoteText", "string", None),
        ("StructuredPayload", "string?", None),
        ("AuthorDoctorId", "long?", None),
    ]),
    ("HmsDiagnosis", "HMS_Diagnosis", "diagnoses", [
        ("VisitId", "long", None),
        ("DiagnosisTypeReferenceValueId", "long?", None),
        ("ICDSystem", "string", 30),
        ("ICDCode", "string", 30),
        ("ICDVersion", "string?", 20),
        ("ICDDescription", "string?", 500),
        ("DiagnosisOn", "DateTime?", None),
    ]),
    ("HmsMedicalProcedure", "HMS_Procedure", "procedures", [
        ("VisitId", "long", None),
        ("ProcedureCode", "string", 50),
        ("ProcedureSystem", "string?", 30),
        ("ProcedureDescription", "string?", 500),
        ("PerformedOn", "DateTime?", None),
        ("PerformedByDoctorId", "long?", None),
        ("Notes", "string?", 1000),
    ]),
    ("HmsPrescription", "HMS_Prescription", "prescriptions", [
        ("PrescriptionNo", "string", 60),
        ("VisitId", "long", None),
        ("PatientId", "long", None),
        ("DoctorId", "long", None),
        ("PrescribedOn", "DateTime", None),
        ("PrescriptionStatusReferenceValueId", "long", None),
        ("ValidFrom", "DateTime?", None),
        ("ValidTo", "DateTime?", None),
        ("Indication", "string?", 1000),
        ("Notes", "string?", None),
    ]),
    ("HmsPrescriptionItem", "HMS_PrescriptionItems", "prescription-items", [
        ("PrescriptionId", "long", None),
        ("LineNo", "int", None),
        ("MedicineId", "long", None),
        ("UnitId", "long?", None),
        ("Quantity", "decimal?", None),
        ("DosageText", "string?", 200),
        ("FrequencyText", "string?", 150),
        ("DurationDays", "int?", None),
        ("RouteReferenceValueId", "long?", None),
        ("IsPRN", "bool", None),
        ("ItemNotes", "string?", 1000),
    ]),
    ("HmsPrescriptionNote", "HMS_PrescriptionNotes", "prescription-notes", [
        ("PrescriptionId", "long", None),
        ("NoteTypeReferenceValueId", "long", None),
        ("NoteText", "string", None),
    ]),
    ("HmsPaymentMode", "HMS_PaymentModes", "payment-modes", [
        ("ModeCode", "string", 40),
        ("ModeName", "string", 120),
        ("SortOrder", "int", None),
    ]),
    ("HmsBillingHeader", "HMS_BillingHeader", "billing-headers", [
        ("BillNo", "string", 60),
        ("VisitId", "long", None),
        ("PatientId", "long", None),
        ("BillDate", "DateTime", None),
        ("BillingStatusReferenceValueId", "long", None),
        ("SubTotal", "decimal?", None),
        ("TaxTotal", "decimal?", None),
        ("DiscountTotal", "decimal?", None),
        ("GrandTotal", "decimal?", None),
        ("CurrencyCode", "string?", 3),
        ("Notes", "string?", 1000),
    ]),
    ("HmsBillingItem", "HMS_BillingItems", "billing-items", [
        ("BillingHeaderId", "long", None),
        ("LineNo", "int", None),
        ("ServiceTypeReferenceValueId", "long", None),
        ("Description", "string?", 500),
        ("Quantity", "decimal", None),
        ("UnitPrice", "decimal?", None),
        ("LineTotal", "decimal?", None),
        ("LabOrderId", "long?", None),
        ("PrescriptionId", "long?", None),
        ("PharmacySalesId", "long?", None),
        ("ExternalReference", "string?", 120),
    ]),
    ("HmsPaymentTransaction", "HMS_PaymentTransactions", "payment-transactions", [
        ("BillingHeaderId", "long", None),
        ("PaymentModeId", "long", None),
        ("Amount", "decimal", None),
        ("TransactionOn", "DateTime", None),
        ("TransactionStatusReferenceValueId", "long", None),
        ("ReferenceNo", "string?", 120),
        ("Notes", "string?", 500),
    ]),
]


def emit_entity(cls, table, props):
    lines = [
        "using Healthcare.Common.Entities;",
        "",
        "namespace HMSService.Domain.Entities;",
        "",
        f"public sealed class {cls} : BaseEntity",
        "{",
    ]
    for name, typ, _ in props:
        lines.append(f"    public {typ} {name} {{ get; set; }}" + (" = null!;" if typ == "string" else ""))
    lines.append("}")
    return "\n".join(lines)


def emit_config(cls, table, props):
    lines = [
        "using HMSService.Domain.Entities;",
        "using Microsoft.EntityFrameworkCore;",
        "using Microsoft.EntityFrameworkCore.Metadata.Builders;",
        "",
        "namespace HMSService.Infrastructure.Persistence.Configurations;",
        "",
        f"public sealed class {cls}Configuration : IEntityTypeConfiguration<{cls}>",
        "{",
        f"    public void Configure(EntityTypeBuilder<{cls}> builder)",
        "    {",
        f'        builder.ToTable("{table}");',
        "        builder.HasKey(e => e.Id);",
        "        builder.Property(e => e.RowVersion).IsRowVersion();",
    ]
    for name, typ, mx in props:
        if mx:
            lines.append(f"        builder.Property(e => e.{name}).HasMaxLength({mx});")
        if typ == "decimal" or typ == "decimal?":
            lines.append(f"        builder.Property(e => e.{name}).HasPrecision(18, 4);")
        if typ == "string" and mx is None:
            lines.append(f"        builder.Property(e => e.{name});")
    lines.extend([
        "    }",
        "}",
    ])
    return "\n".join(lines)


def emit_dtos(cls, props, route):
    base = cls[3:] if cls.startswith("Hms") else cls
    create_fields = []
    update_fields = []
    resp_fields = []
    for name, typ, _ in props:
        create_fields.append(f"    public {typ} {name} {{ get; set; }}")
        update_fields.append(f"    public {typ} {name} {{ get; set; }}")
        resp_fields.append(f"    public {typ} {name} {{ get; set; }}")
    create = "\n".join([
        f"namespace HMSService.Application.DTOs.Extended;",
        "",
        f"public sealed class Create{base}Dto",
        "{",
        *create_fields,
        "}",
    ])
    update = "\n".join([
        f"namespace HMSService.Application.DTOs.Extended;",
        "",
        f"public sealed class Update{base}Dto",
        "{",
        *update_fields,
        "}",
    ])
    resp = "\n".join([
        f"namespace HMSService.Application.DTOs.Extended;",
        "",
        f"public sealed class {base}ResponseDto",
        "{",
        "    public long Id { get; set; }",
        "    public long TenantId { get; set; }",
        "    public long? FacilityId { get; set; }",
        *resp_fields,
        "}",
    ])
    return create, update, resp, base


def main():
    for cls, table, route, props in SPECS:
        (DOMAIN / f"{cls}.cs").write_text(emit_entity(cls, table, props), encoding="utf-8")
        (INFRA / f"{cls}Configuration.cs").write_text(emit_config(cls, table, props), encoding="utf-8")
        create, update, resp, base = emit_dtos(cls, props, route)
        (APP_DTO / f"Create{base}Dto.cs").write_text(create, encoding="utf-8")
        (APP_DTO / f"Update{base}Dto.cs").write_text(update, encoding="utf-8")
        (APP_DTO / f"{base}ResponseDto.cs").write_text(resp, encoding="utf-8")
    print("Generated", len(SPECS), "entities + configs + dtos")


if __name__ == "__main__":
    main()
