# HMS — schema vs code revalidation (2026-03-21)

## Summary

`02_HMS.sql` tables are now represented in **HMSService** as follows.

| Table | Entity | DbSet | REST area |
|-------|--------|-------|-----------|
| HMS_VisitType | `HmsVisitType` | `VisitTypes` | `/api/v1/visit-types` |
| HMS_Appointment | `HmsAppointment` | `Appointments` | existing |
| HMS_Visit | `HmsVisit` | `Visits` | existing |
| HMS_AppointmentStatusHistory | `HmsAppointmentStatusHistory` | `AppointmentStatusHistories` | `/api/v1/appointment-status-history` |
| HMS_AppointmentQueue | `HmsAppointmentQueue` | `AppointmentQueues` | `/api/v1/appointment-queue` |
| HMS_Vitals | `HmsVital` | `Vitals` | `/api/v1/vitals` |
| HMS_ClinicalNotes | `HmsClinicalNote` | `ClinicalNotes` | `/api/v1/clinical-notes` |
| HMS_Diagnosis | `HmsDiagnosis` | `Diagnoses` | `/api/v1/diagnoses` |
| HMS_Procedure | `HmsMedicalProcedure` | `MedicalProcedures` | `/api/v1/procedures` |
| HMS_Prescription | `HmsPrescription` | `Prescriptions` | `/api/v1/prescriptions` |
| HMS_PrescriptionItems | `HmsPrescriptionItem` | `PrescriptionItems` | `/api/v1/prescription-items` |
| HMS_PrescriptionNotes | `HmsPrescriptionNote` | `PrescriptionNotes` | `/api/v1/prescription-notes` |
| HMS_PaymentModes | `HmsPaymentMode` | `PaymentModes` | `/api/v1/payment-modes` |
| HMS_BillingHeader | `HmsBillingHeader` | `BillingHeaders` | `/api/v1/billing-headers` |
| HMS_BillingItems | `HmsBillingItem` | `BillingItems` | `/api/v1/billing-items` |
| HMS_PaymentTransactions | `HmsPaymentTransaction` | `PaymentTransactions` | `/api/v1/payment-transactions` |

## Notes

- **EF relationships**: composite FKs to `Patient`, `Doctor`, `ReferenceDataValue`, etc. are enforced in SQL; entities use scalar FK columns without full relationship graphs (same pattern as incremental delivery).
- **Catalog `FacilityId`**: `HmsPaymentMode` and `HmsVisitType` keep nullable `FacilityId`; `HmsDbContext.SaveChangesAsync` does **not** auto-fill facility for these types (tenant-wide catalog rows).
- **Document numbers**: `PrescriptionNo`, `BillNo`, and queue `QueueToken` can be generated when omitted on create (see `HmsDocumentNumberHelper` / queue service).
- **AutoMapper**: extended maps live in **`HmsMappingProfile`** (merged from a separate profile file to avoid invalid `CreateMap<,>` without `()` before `.ForMember` chains).

## Tests

- Added `HMSService.Tests/Services/VitalsServiceTests.cs` (facility validation + paged call).
- Full solution test count: run `dotnet test HMSService.Tests/HMSService.Tests.csproj`.

## Remaining (platform scope)

- **LIS / LMS / Pharmacy** services: full domain + APIs + tests per original remediation brief (not completed in this pass).
- **Integration tests** with `WebApplicationFactory` + in-memory DB: recommended next step for extended controllers.
