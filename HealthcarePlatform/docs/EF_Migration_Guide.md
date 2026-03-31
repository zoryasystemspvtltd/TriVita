# Entity Framework Core migrations — HealthcarePlatform (TriVita)

Step-by-step guide for creating and applying EF Core migrations against the **unified** SQL Server model (`HealthcareDbContext` in `TriVita.UnifiedDatabase`).

---

## 1. Introduction

TriVita HealthcarePlatform uses **Entity Framework Core 8** with a dedicated project that holds the **single-database, multi-schema** model:

| Item | Location |
|------|----------|
| Migrations project | `TriVita.UnifiedDatabase/TriVita.UnifiedDatabase.csproj` |
| DbContext | `HealthcareDbContext` |
| Design-time factory | `HealthcareDbContextFactory` (used automatically by `dotnet ef`) |
| Solution file | `HealthcarePlatform.sln` |

Migrations live in `TriVita.UnifiedDatabase/Migrations/`. Individual microservices may still use their own `DbContext` types at **runtime**; this guide focuses on the **unified** migration project used to provision or evolve the shared database.

**What `dotnet ef migrations add` does:** compares the current EF model to the last snapshot and generates a new C# migration class (and updates the model snapshot) describing schema changes.

**What `dotnet ef database update` does:** applies pending migrations to the target database, creating or altering objects as defined in those migrations.

---

## 2. Prerequisites

- **.NET SDK** 8.x (or compatible; solution targets `net8.0`).
- **SQL Server** reachable from your machine (LocalDB, Docker SQL Server, Azure SQL, etc.).
- **EF Core CLI tools** (global or local tool manifest):

  ```powershell
  dotnet tool install --global dotnet-ef
  ```

  Align the tool with your EF packages (e.g. **8.0.x** for `Microsoft.EntityFrameworkCore.*` 8.0.11).

- **Restore** the solution before first use:

  ```powershell
  cd I:\Projects\TriVita\HealthcarePlatform
  dotnet restore
  ```

---

## 3. Project Setup

### 3.1 Recommended working directory

Either:

- **Option A (simplest):** `cd` into the unified project folder so `--project` can be omitted or shortened.

  ```powershell
  cd I:\Projects\TriVita\HealthcarePlatform\TriVita.UnifiedDatabase
  ```

- **Option B:** stay at the solution root and pass `--project` explicitly (see sections below).

### 3.2 Connection string (design-time)

`HealthcareDbContextFactory` resolves the connection string in this order:

1. Environment variable **`TRIVITA_UNIFIED_SQL`**
2. If unset, default LocalDB:

   `Server=(localdb)\mssqllocaldb;Database=TriVitaHealthcare;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true`

Optional: **`TRIVITA_USE_MODULE_SCHEMAS=false`** assigns tables to the default schema (`dbo`) instead of module schemas (`hms`, `lms`, etc.). **Do not flip this casually** after migrations already exist; it changes the intended model shape.

### 3.3 `appsettings.json` (optional)

The design-time factory does **not** require `appsettings.json`; it relies on environment variables for CLI operations. For **runtime** (a host API that runs migrations programmatically), use `appsettings.{Environment}.json` and standard `ConnectionStrings` configuration in that host — that is separate from `dotnet ef` design-time.

---

## 4. Initial Migration (First-Time Setup)

Use this when **no migrations exist yet** in `TriVita.UnifiedDatabase`, or you are onboarding a new clone.

### 4.1 From the unified project directory

```powershell
cd I:\Projects\TriVita\HealthcarePlatform\TriVita.UnifiedDatabase

# Optional: point to your SQL Server
$env:TRIVITA_UNIFIED_SQL = "Server=YOUR_SERVER;Database=TriVitaHealthcare;User Id=...;Password=...;TrustServerCertificate=True"

# Create the first migration (choose a descriptive name once per greenfield baseline)
dotnet ef migrations add InitialUnifiedSchema --project TriVita.UnifiedDatabase.csproj

# Create the database (if missing) and apply all migrations
dotnet ef database update --project TriVita.UnifiedDatabase.csproj
```

### 4.2 From the solution root

```powershell
cd I:\Projects\TriVita\HealthcarePlatform

dotnet ef migrations add InitialUnifiedSchema `
  --project TriVita.UnifiedDatabase\TriVita.UnifiedDatabase.csproj

dotnet ef database update `
  --project TriVita.UnifiedDatabase\TriVita.UnifiedDatabase.csproj
```

### 4.3 What you should see

- New files under `TriVita.UnifiedDatabase/Migrations/` (e.g. `{Timestamp}_InitialUnifiedSchema.cs`, `.Designer.cs`, and `HealthcareDbContextModelSnapshot.cs`).
- After `database update`, the target database contains `__EFMigrationsHistory` and all tables/indexes/FKs from the migration.

> **Note:** The repository may already include `InitialUnifiedSchema`. In that case, **skip** `migrations add` and only run `database update` for a new environment, or add a **new** migration with a different name when the model changes.

---

## 5. Incremental Migrations (Ongoing Development)

After you change entity classes or fluent configurations in any referenced Infrastructure project:

1. **Add a migration** with a **clear, unique name** (PascalCase, no spaces):

   ```powershell
   cd I:\Projects\TriVita\HealthcarePlatform\TriVita.UnifiedDatabase

   dotnet ef migrations add AddPatientWalletIndexes --project TriVita.UnifiedDatabase.csproj
   ```

2. **Review** the generated `Up`/`Down` methods. Ensure there are no unintended drops on production-bound databases.

3. **Apply** to your database:

   ```powershell
   dotnet ef database update --project TriVita.UnifiedDatabase.csproj
   ```

4. **Commit** the new migration `.cs` files and the updated `HealthcareDbContextModelSnapshot.cs` to source control.

---

## 6. Updating Database

### 6.1 Apply all pending migrations

```powershell
dotnet ef database update --project TriVita.UnifiedDatabase\TriVita.UnifiedDatabase.csproj
```

### 6.2 Apply up to a specific migration

Useful for staged rollouts or verifying an intermediate state:

```powershell
dotnet ef database update 20260331192050_InitialUnifiedSchema `
  --project TriVita.UnifiedDatabase\TriVita.UnifiedDatabase.csproj
```

Use the migration **class name** or **full migration ID** as shown in `__EFMigrationsHistory` / the migration file name.

### 6.3 List migrations

```powershell
dotnet ef migrations list --project TriVita.UnifiedDatabase\TriVita.UnifiedDatabase.csproj
```

### 6.4 Generate SQL script (review or DBA handoff)

```powershell
dotnet ef migrations script `
  --project TriVita.UnifiedDatabase\TriVita.UnifiedDatabase.csproj `
  --output I:\Projects\TriVita\HealthcarePlatform\docs\generated-migration.sql

# Idempotent script (safe to run multiple times in some scenarios)
dotnet ef migrations script --idempotent `
  --project TriVita.UnifiedDatabase\TriVita.UnifiedDatabase.csproj `
  --output I:\Projects\TriVita\migrations_unified.sql
```

---

## 7. Rollback Migrations

EF Core does not apply “Down” automatically on `database update` to the latest; you **move the database** to an earlier migration, then optionally **remove** the last migration from the project.

### 7.1 Revert database to a previous migration

```powershell
dotnet ef database update <PreviousMigrationNameOrId> `
  --project TriVita.UnifiedDatabase\TriVita.UnifiedDatabase.csproj
```

This runs the `Down` methods of migrations that were applied **after** the target migration.

### 7.2 Remove the last migration (not yet applied to shared environments)

If the migration was **only** created locally and **not** pushed/applied elsewhere:

```powershell
dotnet ef migrations remove --project TriVita.UnifiedDatabase\TriVita.UnifiedDatabase.csproj
```

This deletes the last migration files and reverts the model snapshot.

> **Warning:** If the migration was already applied in **production**, do **not** only `remove` from code. Coordinate a **forward fix** (new migration) or a controlled downgrade with backups and downtime planning.

---

## 8. Multi-Environment Setup (Dev / Test / Prod)

| Concern | Recommendation |
|--------|------------------|
| Connection strings | Use **environment variables** or **User Secrets** / **Key Vault** per environment; never commit production passwords. |
| Dev | LocalDB or shared dev SQL; `TRIVITA_UNIFIED_SQL` in your shell profile or `.env` (if your tooling loads it). |
| Test | Dedicated database; reset or migrate in CI before integration tests. |
| Prod | Run `database update` from a **controlled pipeline** or generated **SQL script** reviewed by DBA; require **backup** first. |
| Schema flag | Keep `TRIVITA_USE_MODULE_SCHEMAS` consistent per environment for a given database; changing it mid-life needs a migration plan. |

Example (PowerShell session for dev):

```powershell
$env:TRIVITA_UNIFIED_SQL = "Server=dev-sql;Database=TriVita_Dev;..."
cd I:\Projects\TriVita\HealthcarePlatform\TriVita.UnifiedDatabase
dotnet ef database update --project TriVita.UnifiedDatabase.csproj
```

---

## 9. Troubleshooting

### 9.1 “No DbContext was found” / design-time factory not used

- Ensure you pass **`--project`** pointing to `TriVita.UnifiedDatabase.csproj`.
- Confirm `HealthcareDbContextFactory` implements `IDesignTimeDbContextFactory<HealthcareDbContext>` and builds successfully (`dotnet build`).
- If the DbContext lived only in another assembly without a factory, add a factory or use **`--startup-project`** so EF can build a host that registers the context.

### 9.2 “More than one DbContext”

Specify the context:

```powershell
dotnet ef database update --context HealthcareDbContext --project TriVita.UnifiedDatabase\TriVita.UnifiedDatabase.csproj
```

### 9.3 Migration conflicts (merge / team)

- Pull latest; if two branches added migrations, **one** branch may need to **remove** their local migration (if not deployed) and re-add after merging snapshot, or merge carefully and fix snapshot conflicts in Git.
- Never hand-edit `HealthcareDbContextModelSnapshot.cs` unless you know EF snapshot rules; prefer `migrations remove` and re-add, or resolve conflicts with team guidance.

### 9.4 Package / restore errors

- Run `dotnet restore` on the solution.
- Match **EF Core** package versions across `TriVita.UnifiedDatabase` and tool: `dotnet-ef` 8.x for EF 8 packages.

### 9.5 Database connection errors

- Verify `TRIVITA_UNIFIED_SQL` (firewall, credentials, `TrustServerCertificate`, encryption settings).
- For LocalDB, ensure the instance is started: `sqllocaldb start mssqllocaldb`.
- Confirm the database name and server match the environment you intend to change.

### 9.6 Build failures when running `dotnet ef`

- `dotnet ef` builds the project first. Fix compile errors in **any** referenced Infrastructure/Domain project.

### 9.7 Decimal / precision warnings

Warnings about `decimal` without precision come from the model. Fix in **`IEntityTypeConfiguration`** (e.g. `HasPrecision`) in the owning service’s Infrastructure project, then add a new migration.

---

## 10. Best Practices

- **Naming:** Use verb-led, descriptive migration names: `AddHmsBillingIndexes`, `AlignLisResultColumnLength`.
- **Versioning:** One logical change set per migration when possible; avoid huge unrelated mixes.
- **Avoid manual DDL** on databases that EF manages — drift causes failed or wrong migrations. If you must hotfix prod, follow up with a migration that matches reality or document an explicit baseline.
- **Backup** production (and critical non-prod) databases **before** `database update` or running generated scripts.
- **Review** generated `Up`/`Down` for accidental `DropTable` / data loss.
- **CI:** Build + optionally `dotnet ef migrations has-pending-model-changes` (or script generation) to catch forgotten migrations.
- **Per-service contexts:** Keep using service-specific `DbContext` instances in APIs for bounded contexts; use **this** project as the **single source of truth** for full-database schema evolution unless your team standardizes differently.

---

## Quick reference — unified project

```powershell
# Paths
$SlnRoot  = "I:\Projects\TriVita\HealthcarePlatform"
$Unified  = "$SlnRoot\TriVita.UnifiedDatabase\TriVita.UnifiedDatabase.csproj"

# Add migration
dotnet ef migrations add <MigrationName> --project $Unified

# Update database
dotnet ef database update --project $Unified

# Remove last migration (local only, not applied upstream)
dotnet ef migrations remove --project $Unified
```

For **startup project** style (when DbContext is not self-contained):

```powershell
dotnet ef migrations add <Name> `
  --project Path\To\Infrastructure.csproj `
  --startup-project Path\To\ApiHost.csproj
```

---

*Document version: aligned with `TriVita.UnifiedDatabase` and `HealthcareDbContext`. Update paths if the solution layout changes.*
