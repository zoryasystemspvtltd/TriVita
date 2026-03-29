# HealthcarePlatform — Schema vs Code Delta Analysis (01)

**Document type:** Evidence-based audit (repository scan, March 2026)  
**Code roots:** `HealthcarePlatform/HMSService`, `LISService`, `LMSService`, `PharmacyService`  
**SQL sources:** `02_HMS.sql`, `03_LIS.sql`, `04_LMS.sql`, `05_Pharmacy.sql` (project root `TriVita/`)

**Method:** Controllers, routes, `*DbContext` `DbSet<>`, domain entity files, and `CREATE TABLE` names were enumerated from the codebase and scripts. No features were inferred that are not present in code.

---

## Summary

| Module | Schema tables (CREATE TABLE) | Domain entities / DbSets | API surface |
|--------|-----------------------------|--------------------------|-------------|
| HMSService | 16 | 16 entities, 16 DbSets | 16 controllers (appointments, visits, visit-types, 13 extended HMS resources) |
| LISService | 15 | 15 entities, 15 DbSets | 16 controllers (15 LIS resources + `InfoController`) |
| LMSService | 10 | 10 entities, 10 DbSets | 11 controllers (10 LMS + `InfoController`) |
| PharmacyService | 20 | 20 entities, 20 DbSets | 21 controllers (20 Pharmacy + `InfoController`) |

**Cross-cutting (all four):** JWT auth (`[Authorize]`), API versioning `api/v{version}/…`, `BaseResponse<T>` responses, tenant middleware, global soft-delete tenant filter on `BaseEntity`, Swagger via `AddTriVitaSwagger`, health endpoint `/health`.

---

## MODULE: HMSService

### Implemented functionalities (from code)

**API — controllers and route prefixes** (`HMSService.API/Controllers`)

| Controller | Route prefix |
|------------|--------------|
| `AppointmentsController` | `api/v{version}/appointments` |
| `VisitsController` | `api/v{version}/visits` |
| `VisitTypesController` | `api/v{version}/visit-types` |
| `AppointmentStatusHistoryController` | `api/v{version}/appointment-status-history` |
| `AppointmentQueueController` | `api/v{version}/appointment-queue` |
| `VitalsController` | `api/v{version}/vitals` |
| `ClinicalNotesController` | `api/v{version}/clinical-notes` |
| `DiagnosesController` | `api/v{version}/diagnoses` |
| `MedicalProceduresController` | `api/v{version}/procedures` |
| `PrescriptionsController` | `api/v{version}/prescriptions` |
| `PrescriptionItemsController` | `api/v{version}/prescription-items` |
| `PrescriptionNotesController` | `api/v{version}/prescription-notes` |
| `PaymentModesController` | `api/v{version}/payment-modes` |
| `BillingHeadersController` | `api/v{version}/billing-headers` |
| `BillingItemsController` | `api/v{version}/billing-items` |
| `PaymentTransactionsController` | `api/v{version}/payment-transactions` |

**Endpoints (pattern):** Each of the extended resource controllers exposes **GET by id**, **GET paged**, **POST**, **PUT**, **DELETE** (soft delete via `IsDeleted` / `HmsCrudServiceBase`), evidenced on `AppointmentsController` and extended controllers. `AppointmentsController` / `VisitsController` add **query filters** on paged GET (e.g. patient/doctor/date ranges) per implementation files.

**Application**

- `IAppointmentService` / `AppointmentService`; `IVisitService` / `VisitService` (dedicated repositories).
- Extended: `IVisitTypeService`, `IAppointmentStatusHistoryService`, `IAppointmentQueueService`, `IVitalService`, `IClinicalNoteService`, `IDiagnosisService`, `IMedicalProcedureService`, `IPrescriptionService`, `IPrescriptionItemService`, `IPrescriptionNoteService`, `IPaymentModeService`, `IBillingHeaderService`, `IBillingItemService`, `IPaymentTransactionService` with matching implementations in `Services/Extended/HmsExtendedEntityServices.cs`.
- `INotificationHelper` — outbound HTTP to CommunicationService (not a table in `02_HMS.sql`).

**Domain**

- Entities under `HMSService.Domain/Entities`: `HmsVisitType`, `HmsAppointment`, `HmsAppointmentStatusHistory`, `HmsAppointmentQueue`, `HmsVisit`, `HmsVital`, `HmsClinicalNote`, `HmsDiagnosis`, `HmsMedicalProcedure`, `HmsPrescription`, `HmsPrescriptionItem`, `HmsPrescriptionNote`, `HmsPaymentMode`, `HmsBillingHeader`, `HmsBillingItem`, `HmsPaymentTransaction` (16 files).

**Infrastructure**

- `HmsDbContext`: `DbSet<>` for each entity above (16 sets).
- `IAppointmentRepository` / `AppointmentRepository`, `IVisitRepository` / `VisitRepository`; generic `IRepository<T>` / `EfRepository<T>` for extended entities.
- EF configurations map to SQL table names (e.g. `HMS_Procedure` for `HmsMedicalProcedure`, `HMS_PrescriptionNotes` for notes).

### Schema (`02_HMS.sql`) — major tables

`HMS_VisitType`, `HMS_Appointment`, `HMS_AppointmentStatusHistory`, `HMS_AppointmentQueue`, `HMS_Visit`, `HMS_Vitals`, `HMS_ClinicalNotes`, `HMS_Diagnosis`, `HMS_Procedure`, `HMS_Prescription`, `HMS_PrescriptionItems`, `HMS_PrescriptionNotes`, `HMS_PaymentModes`, `HMS_BillingHeader`, `HMS_BillingItems`, `HMS_PaymentTransactions`.

### Schema coverage

- **Tables fully used (CRUD API + EF mapping):** All 16 tables listed above have a corresponding entity, configuration `ToTable(...)`, `DbSet`, and REST controller.
- **Tables partially used:** None identified at table level; all mapped tables have CRUD endpoints.
- **Tables not used:** None from the `CREATE TABLE` list in `02_HMS.sql`.

### Gap / delta

- **Missing implementation areas (vs richer domain semantics):** No dedicated **workflow/orchestration** services in code for multi-step clinical or billing processes beyond per-entity CRUD (not required by `CREATE TABLE` alone).
- **Partial implementations:** **EF relationship API** — configurations set keys/columns/table names; **navigation properties / `HasOne`/`WithMany` for FKs** are not required for the implemented scalar-FK CRUD pattern (sample: configs do not declare full relationship graph).
- **Misalignment:** None observed between `ToTable` names and `02_HMS.sql` table names for the listed entities.
- **Out of schema:** Notification integration is **not** persisted in HMS tables.

---

## MODULE: LISService

### Implemented functionalities (from code)

**API** (`LISService.API/Controllers`)

- `InfoController` — `api/v{version}/info` (module metadata).
- Per-table controllers under `Controllers/v1/Entities/`: routes include `test-category`, `sample-type`, `test-master`, `test-parameter`, `test-reference-range`, `lab-order`, `lab-order-item`, `order-status-history`, `sample-collection`, `sample-tracking`, `lab-result`, `result-approval`, `result-history`, `report-header`, `report-detail`.

**Endpoints:** Generated pattern on `TestCategoryController`: **GET** `{id}`, **GET** paged, **POST**, **PUT**, **DELETE**.

**Application**

- `IInfoService` / `InfoService`.
- Generated `*Service` / `I*Service` pairs under `LISService.Application.Services.Entities` (e.g. `LisTestCategoryService` / `ILisTestCategoryService`), using `LisCrudServiceBase`, AutoMapper profiles `LisGeneratedMappingProfile`, optional FluentValidation via `NoOpValidator<T>` fallback.

**Domain**

- 15 entity types matching LIS schema (e.g. `LisTestCategory` … `LisReportDetail`).

**Infrastructure**

- `LisDbContext` — 15 `DbSet<>` entries; `EfRepository<T>`; configurations in `Persistence/Configurations`.

### Schema (`03_LIS.sql`)

15 `CREATE TABLE` statements: `LIS_TestCategory` through `LIS_ReportDetails` (as listed in grep of script).

### Schema coverage

- **Tables fully used:** All 15 LIS tables have entity + `DbSet` + controller + service.
- **Tables partially used:** None.
- **Tables not used:** None from `03_LIS.sql` table list.

### Gap / delta

- **Missing implementation areas:** **EF Core relationship configuration** (FKs as **scalar properties** only; no evidence of full navigation model in sample `LisLabOrderConfiguration`).
- **Partial implementations:** **Business rules** beyond CRUD (e.g. lab workflow state machine) — **not present** as separate services; only generic CRUD.
- **Misalignment:** None identified between table names in SQL and `ToTable(...)` for generated configs (generator-aligned).

---

## MODULE: LMSService

### Implemented functionalities (from code)

**API**

- `InfoController` — `api/v{version}/info`.
- Entities controllers: `processing-stage`, `work-queue`, `technician-assignment`, `equipment`, `equipment-maintenance`, `equipment-calibration`, `qc-record`, `qc-result`, `lab-inventory`, `lab-inventory-transaction`.

**Endpoints:** Same CRUD pattern as LIS (GET by id, GET paged, POST, PUT, DELETE).

**Application / Domain / Infrastructure**

- `LmsCrudServiceBase`, generated services in `LMSService.Application.Services.Entities`, `LmsDbContext` with 10 `DbSet<>`, 10 domain entities, `EfRepository<T>`.

### Schema (`04_LMS.sql`)

10 tables: `ProcessingStages`, `WorkQueue`, `TechnicianAssignment`, `Equipment`, `EquipmentMaintenance`, `EquipmentCalibration`, `QCRecords`, `QCResults`, `LabInventory`, `LabInventoryTransactions`.

### Schema coverage

- **Tables fully used:** All 10.
- **Tables partially used:** None.
- **Tables not used:** None.

### Gap / delta

- **Missing implementation areas:** **Queue/workflow orchestration** (e.g. automatic stage transitions) — **not implemented**; only CRUD on operational tables.
- **Partial implementations:** Same as LIS: **navigation relationships** in EF optional; scalars present.
- **Misalignment:** None identified.

---

## MODULE: PharmacyService

### Implemented functionalities (from code)

**API**

- `InfoController` — `api/v{version}/info`.
- Controllers: `medicine-category`, `manufacturer`, `composition`, `medicine`, `medicine-composition`, `medicine-batch`, `batch-stock`, `stock-ledger`, `purchase-order`, `purchase-order-item`, `goods-receipt`, `goods-receipt-item`, `pharmacy-sale`, `pharmacy-sales-item`, `prescription-mapping`, `stock-adjustment`, `stock-adjustment-item`, `stock-transfer`, `stock-transfer-item`, `expiry-tracking`.

**Application / Domain / Infrastructure**

- `PhrCrudServiceBase`, generated services, `PharmacyDbContext` with 20 `DbSet<>`, 20 entities, `EfRepository<T>`.

### Schema (`05_Pharmacy.sql`)

20 `CREATE TABLE` statements (MedicineCategory … ExpiryTracking).

### Schema coverage

- **Tables fully used:** All 20.
- **Tables partially used:** None.
- **Tables not used:** None.

### Gap / delta

- **Missing implementation areas:** **Stock movement orchestration** (atomic updates across `BatchStock`, `StockLedger`, sales lines) — **not implemented** as transactional domain services; endpoints are **per-entity CRUD** only.
- **Partial implementations:** **Cross-table transactional consistency** left to callers / future domain services.
- **Misalignment:** None identified for table mapping.

---

## Global delta (all modules)

1. **SQL constructs outside tables:** Indexes, constraints, and cross-database FKs in scripts are not separately “implemented” in C# beyond what EF enforces through mappings and SaveChanges behavior.
2. **Test / auth integration:** `IntegrationTest:UseTestAuth` and `TestAuthenticationHandler` are **test/configuration** paths, not schema features.
3. **Coverage metrics:** Unit/integration test counts and line coverage are **not** part of this document (see test projects under each `*.Tests`).

---

*End of analysis 01.*
