# HealthcarePlatform — Schema vs Code Delta Analysis

> **⚠️ Historical snapshot (superseded for LIS / LMS / Pharmacy):** This audit reflected an earlier codebase where LIS/LMS/Pharmacy had only metadata endpoints. The current implementation maps **all major tables** from `03_LIS.sql`, `04_LMS.sql`, and `05_Pharmacy.sql` (entities, EF, CRUD APIs). See **`HealthcarePlatform_Revalidation_Report.md`** for the up-to-date schema vs code summary and test/logging notes.

**Document type:** Evidence-based audit (codebase + SQL scripts)  
**Modules:** HMSService, LISService, LMSService, PharmacyService  
**Schemas:** 02_HMS.sql, 03_LIS.sql, 04_LMS.sql, 05_Pharmacy.sql  

---

## MODULE: HMSService

**Schemas considered:** 02_HMS.sql

### Implemented Functionalities (from code only)

**API (HMSService.API)**

- **AppointmentsController** (`api/v{version}/appointments`): GET by id, GET paged (filters: patientId, doctorId, scheduledFrom, scheduledTo), POST create, PUT update, DELETE soft-delete.
- **VisitsController** (`api/v{version}/visits`): GET by id, GET paged (patientId, doctorId, visitFrom, visitTo), POST create, PUT update, DELETE soft-delete.
- Controllers use `[Authorize]` (JWT as configured).
- **Health:** `/health` with DbContext health check (infrastructure, not domain logic).

**Application**

- **IAppointmentService / AppointmentService:** get by id, paged list, create, update, soft-delete; facility validation where applicable; integrates **INotificationHelper** after appointment create (HTTP to CommunicationService — not 02_HMS tables).
- **IVisitService / VisitService:** get by id, paged list, create, update, soft-delete.
- **INotificationHelper / NotificationHelper:** outbound notification via **INotificationApiClient**.
- DTOs, FluentValidation, AutoMapper, AuditHelper.

**Domain**

- Entities: **HmsAppointment**, **HmsVisit**, **HmsVisitType**.
- Repositories: **IAppointmentRepository**, **IVisitRepository**, generic **IRepository&lt;T&gt;**.

**Infrastructure**

- **HmsDbContext:** DbSets for **HmsVisitType**, **HmsAppointment**, **HmsVisit** only; global tenant filter.
- EF maps to: **HMS_VisitType**, **HMS_Appointment**, **HMS_Visit**.
- **AppointmentRepository**, **VisitRepository:** EF/LINQ only.

**02_HMS.sql — major table groups**

- Scheduling: HMS_VisitType, HMS_Appointment, HMS_AppointmentStatusHistory, HMS_AppointmentQueue, HMS_Visit
- Clinical: HMS_Vitals, HMS_ClinicalNotes, HMS_Diagnosis, HMS_Procedure
- Rx: HMS_Prescription, HMS_PrescriptionItems, HMS_PrescriptionNotes
- Billing: HMS_PaymentModes, HMS_BillingHeader, HMS_BillingItems, HMS_PaymentTransactions

### Schema Coverage

| Category | Tables |
|----------|--------|
| **Tables fully used (CRUD in this service)** | HMS_Appointment, HMS_Visit |
| **Tables partially used** | HMS_VisitType — mapped; FK from appointment/visit only; no API to manage catalog (rows must exist externally). |
| **Tables not used** | HMS_AppointmentStatusHistory, HMS_AppointmentQueue, HMS_Vitals, HMS_ClinicalNotes, HMS_Diagnosis, HMS_Procedure, HMS_Prescription, HMS_PrescriptionItems, HMS_PrescriptionNotes, HMS_PaymentModes, HMS_BillingHeader, HMS_BillingItems, HMS_PaymentTransactions |

### Gap / Delta

- **Missing implementation areas:** Status history, appointment queue, vitals, clinical notes, diagnosis, procedures, HMS prescriptions, prescription notes, payment modes, billing headers/items, payment transactions — no entities, repositories, or endpoints.
- **Partial implementations:** HMS_VisitType — reference data only; no catalog CRUD API.
- **Misalignment:** For HMS_Appointment and HMS_Visit, entities align with 02_HMS columns for those tables.
- **Out of schema:** NotificationHelper → CommunicationService (not 02_HMS persistence).

---

## MODULE: LISService

**Schemas considered:** 03_LIS.sql

### Implemented Functionalities (from code only)

**API**

- **InfoController** only: GET `api/v{version}/info` — static LIS module metadata.

**Application**

- **InfoService:** returns fixed Service/Version/Module strings.
- **LisNotificationHelper** (if present): **INotificationApiClient** only — not LIS database.

**Domain**

- No domain entity source files (excluding build artifacts).

**Infrastructure**

- **LisDbContext:** no DbSet&lt;&gt;; placeholder for future entities.
- DI: LisDbContext + HttpClient for notification client — no LIS table persistence.

**03_LIS.sql — major tables**

LIS_TestCategory, LIS_SampleType, LIS_TestMaster, LIS_TestParameters, LIS_TestReferenceRanges, LIS_LabOrder, LIS_LabOrderItems, LIS_OrderStatusHistory, LIS_SampleCollection, LIS_SampleTracking, LIS_LabResults, LIS_ResultApproval, LIS_ResultHistory, LIS_ReportHeader, LIS_ReportDetails.

### Schema Coverage

| Category | Tables |
|----------|--------|
| **Tables fully used** | None (no EF entities; no queries against LIS tables). |
| **Tables partially used** | None |
| **Tables not used** | All tables in 03_LIS.sql |

### Gap / Delta

- **Missing implementation areas:** Entire LIS domain per 03_LIS.sql.
- **Partial implementations:** None (metadata endpoint only).
- **Misalignment:** LisDbContext registered but empty model vs 03_LIS.sql.

---

## MODULE: LMSService

**Schemas considered:** 04_LMS.sql

### Implemented Functionalities (from code only)

**API**

- **InfoController:** GET `api/v{version}/info` — static LMS metadata.

**Application**

- **InfoService:** fixed LMSService / LMS / 1.0.
- **LmsNotificationHelper** (if present): INotificationApiClient only — not LMS DB.

**Domain**

- No domain entity source files.

**Infrastructure**

- **LmsDbContext:** no DbSet&lt;&gt;; placeholder.
- DI: LmsDbContext + optional notification HTTP — no LMS table access.

**04_LMS.sql — major tables**

ProcessingStages, WorkQueue, TechnicianAssignment, Equipment, EquipmentMaintenance, EquipmentCalibration, QCRecords, QCResults, LabInventory, LabInventoryTransactions.

### Schema Coverage

| Category | Tables |
|----------|--------|
| **Tables fully used** | None |
| **Tables partially used** | None |
| **Tables not used** | All tables in 04_LMS.sql |

### Gap / Delta

- **Missing implementation areas:** Queue, technicians, equipment, QC, inventory — none implemented.
- **Partial implementations:** None.
- **Misalignment:** LmsDbContext empty vs 04_LMS.sql.

---

## MODULE: PharmacyService

**Schemas considered:** 05_Pharmacy.sql

### Implemented Functionalities (from code only)

**API**

- **InfoController:** GET `api/v{version}/info` — static Pharmacy metadata.

**Application**

- **InfoService:** fixed PharmacyService / Pharmacy / 1.0.
- **PharmacyNotificationHelper** (if present): INotificationApiClient only — not Pharmacy DB.

**Domain**

- No domain entity source files.

**Infrastructure**

- **PharmacyDbContext:** no DbSet&lt;&gt;; placeholder.
- DI: PharmacyDbContext + notification HTTP — no pharmacy schema entities.

**05_Pharmacy.sql — major tables**

MedicineCategory, Manufacturer, Composition, Medicine, MedicineComposition, MedicineBatch, BatchStock, StockLedger, PurchaseOrder, PurchaseOrderItems, GoodsReceipt, GoodsReceiptItems, PharmacySales, PharmacySalesItems, PrescriptionMapping, StockAdjustment, StockAdjustmentItems, StockTransfer, StockTransferItems, ExpiryTracking.

### Schema Coverage

| Category | Tables |
|----------|--------|
| **Tables fully used** | None |
| **Tables partially used** | None |
| **Tables not used** | All tables in 05_Pharmacy.sql |

### Gap / Delta

- **Missing implementation areas:** Catalog, stock, procurement, sales, transfers, expiry — none in code.
- **Partial implementations:** None.
- **Misalignment:** PharmacyDbContext empty vs 05_Pharmacy.sql.

---

## Summary Table

| Module | Primary API | DB-backed vs schema script |
|--------|-------------|----------------------------|
| HMSService | Appointments + Visits CRUD + health + notification hook | 2 tables full CRUD; 1 table FK-only; remainder of 02_HMS unused |
| LISService | Info only | 0 LIS tables |
| LMSService | Info only | 0 LMS tables |
| PharmacyService | Info only | 0 Pharmacy tables |

---

*Generated from repository analysis. Features not present in code are not assumed.*
