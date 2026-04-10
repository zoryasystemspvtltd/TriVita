# TriVita Healthcare Platform (.NET 8)

## Swagger (OpenAPI)

All API projects reference **`BuildingBlocks/Healthcare.Swagger`**, which registers:

- **Swashbuckle** + **annotations** (`[SwaggerOperation]`, `[SwaggerTag]`, `[SwaggerResponse]`, …)
- **JWT Bearer** security definition (use **Authorize** in Swagger UI with a token from **IdentityService**)
- **XML documentation** from the API and Application assemblies (`GenerateDocumentationFile` + copy of `*.Application.xml` into the API output)

In **Development**, each service calls **`UseTriVitaSwaggerUi("v1", "…")`** so Swagger UI is grouped as **v1** with request duration display.

## Unit tests

| Test project | Sample coverage |
|--------------|-----------------|
| `HMSService/HMSService.Tests` | `AppointmentServiceTests`, `AppointmentsControllerTests`; `Repositories/` + `Integration/` README templates |
| `LISService/LISService.Tests` | `InfoServiceTests`, `InfoControllerTests` |
| `LMSService/LMSService.Tests` | same pattern |
| `PharmacyService/PharmacyService.Tests` | same pattern |

Packages: **xUnit**, **Moq**, **FluentAssertions**, **Microsoft.NET.Test.Sdk**, **coverlet.collector**.

Run (example):

```bash
dotnet test HMSService/HMSService.Tests/HMSService.Tests.csproj
dotnet test LISService/LISService.Tests/LISService.Tests.csproj
```

## Layout

| Area | Description |
|------|-------------|
| `BuildingBlocks/Healthcare.Common` | **BaseEntity**, **BaseResponse** / **PagedResponse**, **ITenantContext** / **HttpTenantContext**, **GlobalExceptionMiddleware**, **RequireTenantContextMiddleware**, **IEventPublisher** + **NoOpEventPublisher** |
| `BuildingBlocks/Healthcare.Swagger` | Shared **AddTriVitaSwagger** / **UseTriVitaSwaggerUi** for all microservices |
| `HMSService/` | Full HMS slice (**Appointment**, **Visit**) — reference implementation |
| `LISService/`, `LMSService/`, `PharmacyService/`, `SharedService/` | Same Clean Architecture layers; empty `DbContext` + **`GET api/v1/info`** template (extend with your SQL schemas) |
| `IdentityService/` | Issues **JWT** (`sub`, `email`, `tenant_id`, `facility_id`, `role`); other services **validate** the same `Jwt:*` settings |

Each microservice has its own **`.sln`** (`{Service}/{Service}.sln`). The root **`HealthcarePlatform.sln`** includes **all** API/Application/Domain/Infrastructure/Contracts projects.

## Ports (default launchSettings)

| Service | HTTP | HTTPS |
|---------|------|-------|
| HMSService | 5146 | 7029 |
| LISService | 5150 | 7030 |
| LMSService | 5151 | 7031 |
| PharmacyService | 5152 | 7032 |
| SharedService | 5153 | 7033 |
| IdentityService | 5160 | 7040 |

## Shared configuration

Use the **same** values across services for interop:

- **`Jwt:Issuer`** — e.g. `TriVita.Identity`
- **`Jwt:Audience`** — e.g. `TriVita.Services`
- **`Jwt:Key`** — symmetric key, **≥ 32 characters** (override via environment variables in production)

**IdentityService** (token):

- `POST /api/v1/auth/token` — body `{ "email": "...", "password": "..." }` — **[AllowAnonymous]**
- `GET /api/v1/auth/me` — **[Authorize]**

After **IdentityService** starts once against the database, a **demo user** is seeded: email **`admin@demo.local`**, password **`ChangeMe!123`**, **Tenant ID `1`** (required on the portal login form), `facility_id=1`, role **Admin**. Tables are under the **`identity`** schema when using the unified multi-schema database (`identity.Identity_Users`, etc.).

**Other services** (data APIs):

- Require **`Authorization: Bearer`** + **`X-Tenant-Id`** (or JWT claim `tenant_id`). Use **`X-Facility-Id`** / claim `facility_id` where applicable.
- **`GET /health`** — no tenant required.
- **`GET /api/v1/info`** — template endpoint returning service metadata.

## Connection string keys

| Service | Key |
|---------|-----|
| HMS | `HmsDatabase` |
| LIS | `LisDatabase` |
| LMS | `LmsDatabase` |
| Pharmacy | `PharmacyDatabase` |
| Shared | `SharedDatabase` |
| Identity | `IdentityDatabase` |

## Docker (per service)

From `HealthcarePlatform` root:

```bash
docker build -f HMSService/Dockerfile -t hms-service .
docker build -f LISService/Dockerfile -t lis-service .
docker build -f LMSService/Dockerfile -t lms-service .
docker build -f PharmacyService/Dockerfile -t pharmacy-service .
docker build -f SharedService/Dockerfile -t shared-service .
docker build -f IdentityService/Dockerfile -t identity-service .
```

## Next steps

- Map **EF Core** entities to your existing SQL scripts (LIS/LMS/Pharmacy/Shared modules).
- Add **HttpClient/Refit** clients + DTOs in `*.Contracts` (no cross-database access).
- Replace **`EnsureCreated`** seeding in Identity with migrations + proper user provisioning for production.
