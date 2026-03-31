# TriVita HealthcarePlatform — Unified EF Core database

This document describes the single-database Entity Framework Core model that consolidates all microservice domains into one SQL Server database with **per-module schemas**.

## Location

| Item | Path |
|------|------|
| Project | `HealthcarePlatform/TriVita.UnifiedDatabase/TriVita.UnifiedDatabase.csproj` |
| Unified context | `HealthcareDbContext` (partial: `HealthcareDbContext.cs`, `HealthcareDbContext.DbSets.cs`) |
| Schema assignment | `ModuleSchemaExtensions.ApplyModuleSchemas` |
| Design-time factory | `HealthcareDbContextFactory` (`IDesignTimeDbContextFactory<HealthcareDbContext>`) |
| Migrations | `HealthcarePlatform/TriVita.UnifiedDatabase/Migrations/` |

The project is registered in `HealthcarePlatform/HealthcarePlatform.sln`.

## Schemas

When `TriVitaUnifiedModelOptions.UseModuleSchemas` is `true` (default for greenfield and for the shipped initial migration), CLR namespaces map to SQL schemas:

| Prefix | Schema |
|--------|--------|
| `HMSService.Domain` | `hms` |
| `LMSService.Domain` | `lms` |
| `LISService.Domain` | `lis` |
| `PharmacyService.Domain` | `pharmacy` |
| `SharedService.Domain` | `shared` |
| `IdentityService.Domain` | `identity` |
| `CommunicationService.Domain` | `communication` |

Fluent configurations from each `*.Infrastructure` assembly are applied unchanged; schema is set **after** `ApplyConfigurationsFromAssembly`, so table names and column mappings stay as defined in each service.

## Backward compatibility (existing databases)

**Important:** If you already created objects in **`dbo`** from hand-written SQL scripts, this unified migration creates **additional** tables under **`hms` / `lms` / …** — it does **not** rename or move existing `dbo` tables. Running `database update` against a database that already holds the same logical tables in `dbo` can cause duplication or confusion.

- **Greenfield / new database:** Use module schemas (default). Connection string + `dotnet ef database update` as below.
- **Brownfield / script-first `dbo`:** Either  
  - set `TRIVITA_USE_MODULE_SCHEMAS=false`, regenerate migrations from a clean baseline (team decision), or  
  - keep using per-service contexts against the legacy layout and treat this project as reference-only until a data migration plan exists.

Per-service `DbContext` classes in each API are unchanged; this project is primarily for **migrations and optional** consolidated tooling.

## Configuration and tenant behavior

- **Design-time tenant:** `MigrationTenantContext` supplies fixed `TenantId` / `FacilityId` for model building and migrations only.
- **Query filters:** Global filters on `!IsDeleted` and `TenantId` match the original HMS, LIS, LMS, Pharmacy, and Communication contexts. **Shared** enterprise entities use `AuditedEntityBase` (not `BaseEntity`) and are **not** given that filter here, consistent with `SharedDbContext`. **Identity** entities are configured without the HMS-style filter.
- **SaveChanges:** `TenantId` is set on insert for `BaseEntity`; `FacilityId` is filled from `ITenantContext` when appropriate, with the same catalog/IAM exceptions as the individual services (see `PreservesOptionalNullFacility` in `HealthcareDbContext.cs`).

## Environment variables (design-time / CLI)

| Variable | Purpose |
|----------|---------|
| `TRIVITA_UNIFIED_SQL` | SQL Server connection string. If unset, LocalDB is used: `Server=(localdb)\mssqllocaldb;Database=TriVitaHealthcare;...` |
| `TRIVITA_USE_MODULE_SCHEMAS` | Set to `false` to build the model with default schema (`dbo`) for all mapped entities. **Changing this after migrations exist requires a new migration strategy** — do not toggle casually on an existing migrated database. |

## Commands

From the unified project directory:

```powershell
cd I:\Projects\TriVita\HealthcarePlatform\TriVita.UnifiedDatabase

# Add a new migration (after model changes)
dotnet ef migrations add <MigrationName> --project TriVita.UnifiedDatabase.csproj

# Apply migrations
dotnet ef database update --project TriVita.UnifiedDatabase.csproj

# Optional: generate idempotent SQL script
dotnet ef migrations script --project TriVita.UnifiedDatabase.csproj --output I:\Projects\TriVita\migrations_unified.sql
```

Ensure the EF CLI is available: `dotnet tool install --global dotnet-ef` (or use a manifest tool version aligned with EF 8.0.11).

## Initial migration and validation (LocalDB)

The first migration added in this setup:

- **Name:** `InitialUnifiedSchema`  
- **Files:** `Migrations/20260331192050_InitialUnifiedSchema.cs`, matching `.Designer.cs`, and `HealthcareDbContextModelSnapshot.cs`

After `dotnet ef database update` against LocalDB, schema inventory was:

| Schema | Approx. table count |
|--------|---------------------|
| `hms` | 37 |
| `lms` | 66 |
| `lis` | 23 |
| `pharmacy` | 26 |
| `shared` | 14 |
| `identity` | 11 |
| `communication` | 6 |
| `dbo` | 1 (`__EFMigrationsHistory`) |

## EF warnings (decimals)

`dotnet ef` may warn that some `decimal` properties have no explicit precision. Those mappings live in the existing `IEntityTypeConfiguration` classes in each service’s Infrastructure project. Tightening `HasPrecision` / `HasColumnType` there will clear the warnings for future migrations.

## Summary

- **Single database**, **seven module schemas** plus `dbo` for EF history.  
- **One `HealthcareDbContext`** with all `DbSet` properties and configurations merged from all services.  
- **Safe stance toward legacy data:** no automatic drops; brownfield requires an explicit alignment strategy between `dbo` scripts and module schemas.
