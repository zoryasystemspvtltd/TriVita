# HealthcarePlatform — Schema vs Code Revalidation Report

**Date:** 2026-03-21  
**Scope:** HMSService, LISService, LMSService, PharmacyService  
**SQL sources:** `02_HMS.sql`, `03_LIS.sql`, `04_LMS.sql`, `05_Pharmacy.sql`

---

## Executive summary

The original **HealthcarePlatform_Schema_Code_Delta_Analysis.md** described an older snapshot (LIS/LMS/Pharmacy as “info only”, HMS as appointments/visits only). The codebase has since been aligned with the SQL scripts:

| Module | Schema tables (approx.) | Implementation |
|--------|---------------------------|----------------|
| **HMSService** | 16 in `02_HMS.sql` | Entities + EF configs + `HmsDbContext` DbSets + extended CRUD (appointments, visits, clinical, Rx, billing, etc.) + REST |
| **LISService** | 15 in `03_LIS.sql` | Generated + hand-wired domain, `LisDbContext`, CRUD services/controllers per table |
| **LMSService** | 10 in `04_LMS.sql` | Same pattern for LMS operational tables |
| **PharmacyService** | 20 in `05_Pharmacy.sql` | Same pattern for catalog, stock, procurement, sales, transfers, expiry |

**EF relationships:** Foreign keys are enforced in SQL; generated code uses scalar FK columns on entities (consistent with HMS extended pattern). Full relationship navigation graphs can be added incrementally without breaking APIs.

---

## Cross-cutting updates (this remediation)

1. **Structured logging**
   - `RequestLoggingMiddleware` — HTTP method, path, status, `TenantId` / `FacilityId` for `/api/*` after tenant middleware.
   - `GlobalExceptionMiddleware` — logs `Path`, `TraceId`, `TenantId`, `FacilityId` on unhandled exceptions.

2. **Integration testing**
   - `IntegrationTest:UseTestAuth=true` + `TestAuthenticationHandler` (claims: `tenant_id`, `facility_id`) for `WebApplicationFactory` tests.
   - EF Core **InMemory** database swapped in test factories.
   - `public partial class Program` for `WebApplicationFactory<Program>`.

3. **FluentValidation DI (LIS / LMS / Pharmacy)**
   - Open-generic `NoOpValidator<T>` registered before `AddValidatorsFromAssembly` so generated CRUD services resolve `IValidator<T>` when no rules exist yet.

4. **Swagger**
   - Unchanged shared behavior: `AddTriVitaSwagger` (JWT Bearer, XML comments when `.xml` present, annotations).

---

## Test matrix

| Project | Unit tests | Integration tests |
|---------|------------|-------------------|
| HMSService.Tests | Services (e.g. Vitals, Appointments), Controllers | Swagger JSON + Visit Types paged API |
| LISService.Tests | Info, LisTestCategory service, TestCategory controller | Swagger + test-category paged |
| LMSService.Tests | LmsProcessingStage service | Swagger + processing-stage paged |
| PharmacyService.Tests | PhrMedicineCategory service | Swagger + medicine-category paged |

**Coverage:** Run `dotnet test /p:CollectCoverage=true` (coverlet) per test project to measure line coverage; targets (≥90% services, ≥80% controllers) are aspirational and depend on ongoing test additions.

---

## Remaining / optional gaps

- **HMS**: Operational “workflow” logic beyond CRUD (e.g. billing state machines) is **not** in scope unless specified in SQL as constraints only.
- **LIS / LMS / Pharmacy**: Add explicit **FluentValidation** rule classes per DTO where business rules exceed schema constraints.
- **Integration**: Optional **Testcontainers** SQL Server for closer-to-prod tests than InMemory.

---

*This report supersedes the “all tables unused” conclusions in the historical delta doc for LIS/LMS/Pharmacy and reflects the repository state at revalidation time.*
