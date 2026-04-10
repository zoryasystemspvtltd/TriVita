# Local IIS deployment — TriVita HealthcarePlatform

This guide matches the layout under `I:\Projects\PROD\TriVita` and the automation in `deploy/iis/`.

## What gets deployed

| Layer | Source | Output |
|--------|--------|--------|
| APIs | `*.API` projects (seven services) | `I:\Projects\PROD\TriVita\services\<ServiceName>` |
| Portal | `triVita-portal` (`npm run build`) | `I:\Projects\PROD\TriVita\portal` |
| **Excluded** | `TriVita.UnifiedDatabase` | Not published (migrations-only project) |

## Prerequisites (Windows)

1. **IIS** — Turn Windows features on: *Internet Information Services* (World Wide Web services, Management Tools).
2. **ASP.NET Core Hosting Bundle** (match runtime, e.g. .NET 8): [Download](https://dotnet.microsoft.com/download/dotnet) → Hosting Bundle. Install after IIS.
3. **URL Rewrite Module** for IIS: [IIS URL Rewrite](https://www.iis.net/downloads/microsoft/url-rewrite).
4. **Node.js** (for building the portal on the build machine).
5. **SQL Server** (or LocalDB) reachable using the connection strings in each API’s `appsettings.json` / environment overrides.

## One-step publish (build machine)

From an elevated or normal PowerShell (no admin required for publish):

```powershell
Set-ExecutionPolicy -Scope Process Bypass -Force
& 'I:\Projects\TriVita\HealthcarePlatform\deploy\iis\Publish-TriVita.ps1'
```

Optional parameters:

```powershell
& '...\Publish-TriVita.ps1' -ProdRoot 'D:\Deploy\TriVita' -RepoRoot 'I:\Projects\TriVita'
```

## Redeploy after code updates (same folders, preserve config)

Use **`deploy/iis/Redeploy-TriVita.ps1`** when `I:\Projects\PROD\TriVita` already exists. It does **not** delete folders or change IIS; it rebuilds each API, republishes to the same `services\<Name>` path, restores any existing **`appsettings.json`**, **`appsettings.Production.json`**, **`appsettings.Local.json`**, and **`appsettings.Development.json`** from before the publish, rebuilds the portal, restores **`portal\web.config`**, and optionally runs **`iisreset`** (requires Administrator).

```powershell
Set-ExecutionPolicy -Scope Process Bypass -Force
# Recycle IIS (run PowerShell as Administrator if prompted)
& 'I:\Projects\TriVita\HealthcarePlatform\deploy\iis\Redeploy-TriVita.ps1'
# Or skip portal build or IIS reset:
& '...\Redeploy-TriVita.ps1' -SkipPortal
& '...\Redeploy-TriVita.ps1' -SkipIisReset
```

**Swagger in Production:** HMS, LIS, LMS, Pharmacy, and Communication only mount Swagger UI when `ASPNETCORE_ENVIRONMENT` is **Development**, so **`/swagger`** may return **404** under Production. **Identity** and **Shared** still expose Swagger UI in Production. Validate APIs with **`/health`** for every service.

The **Publish-TriVita.ps1** script (first-time / full copy):

- Runs `dotnet publish -c Release` for each API (not UnifiedDatabase).
- Runs `npm install` and `npm run build` in `triVita-portal`.
- Copies `triVita-portal/dist` to `<ProdRoot>\portal` (includes `public\web.config` for SPA routing).

## IIS layout (target)

### Application pools (no managed code)

Create seven pools; set **.NET CLR version** to **No Managed Code**, **Managed pipeline** to **Integrated**:

| App pool | Used by |
|----------|---------|
| TriVita-HMS | `/hms` |
| TriVita-LMS | `/lms` |
| TriVita-LIS | `/lis` |
| TriVita-Pharmacy | `/pharmacy` |
| TriVita-Shared | `/shared` |
| TriVita-Identity | `/identity` |
| TriVita-Communication | `/communication` |

### Site: TriVita-API (port 80)

- **Physical path:** `I:\Projects\PROD\TriVita\api-root` (placeholder; created by publish script).
- **Bindings:** `http`, port **80**, host empty (or your hostname).

Under this site, add **applications** (not virtual directories):

| Path alias | Physical path |
|------------|----------------|
| `/hms` | `I:\Projects\PROD\TriVita\services\HMSService` |
| `/lms` | `I:\Projects\PROD\TriVita\services\LMSService` |
| `/lis` | `I:\Projects\PROD\TriVita\services\LISService` |
| `/pharmacy` | `I:\Projects\PROD\TriVita\services\PharmacyService` |
| `/shared` | `I:\Projects\PROD\TriVita\services\SharedService` |
| `/identity` | `I:\Projects\PROD\TriVita\services\IdentityService` |
| `/communication` | `I:\Projects\PROD\TriVita\services\CommunicationService` |

Assign the matching app pool to each application.

**Port 80 conflict:** If **Default Web Site** already uses port 80, stop it or change its binding before creating **TriVita-API**.

### Site: TriVita-Portal (port 3000)

- **Physical path:** `I:\Projects\PROD\TriVita\portal`
- **Binding:** `http`, port **3000**

Default app pool is fine for static files; ensure **default document** includes `index.html`.

## Automated IIS creation (administrator)

```powershell
# Run PowerShell as Administrator
Set-ExecutionPolicy -Scope Process Bypass -Force
& 'I:\Projects\TriVita\HealthcarePlatform\deploy\iis\Setup-IIS.ps1' -ProdRoot 'I:\Projects\PROD\TriVita'
```

Run **after** `Publish-TriVita.ps1`.

## Frontend API URLs (production build)

`triVita-portal\.env.production` sets bases for local IIS:

- `http://localhost/identity`, `http://localhost/hms`, … (port **80**).

Rebuild the portal after changing these values.

## Backend production configuration

Each API includes `appsettings.Production.json` with:

- **`IIS:PathBase`** — `/hms`, `/lms`, etc., so routes work under IIS applications.
- **`Cors:AllowedOrigins`** — `http://localhost:3000` (portal).
- **`DisableHttpsRedirection`** — `true` for typical HTTP-only local IIS.
- **Cross-service `BaseUrl`** values — `http://localhost/shared`, `http://localhost/lms`, `http://localhost/communication` where applicable.

Set `ASPNETCORE_ENVIRONMENT=Production` (dotnet publish `web.config` usually does this) so these files load.

Override **connection strings** and **JWT** secrets via environment variables or `appsettings.Production.json` on the server (do not use dev keys in real environments).

## Validation checklist

| Check | URL / action |
|-------|----------------|
| HMS health | `GET http://localhost/hms/health` |
| Identity health | `GET http://localhost/identity/health` |
| Portal | Open `http://localhost:3000` |
| Login | Use seeded identity user (see Identity service docs / seeder) |
| CORS | Browser dev tools: no blocked preflight from `:3000` to `:80` |

## Troubleshooting

| Issue | Mitigation |
|--------|------------|
| 502.5 / ANCM error | Install/repair **Hosting Bundle**; check Windows Event Log → *IIS AspNetCore Module*. |
| 404 on `/hms/api/...` | Confirm **PathBase** in Production config matches IIS app alias (leading `/`, no trailing slash). |
| 404 on React deep links | Confirm `web.config` rewrite exists in `portal` and **URL Rewrite** is installed. |
| CORS errors | Confirm portal origin is listed in `Cors:AllowedOrigins`; rebuild portal with correct `VITE_*` bases. |
| SQL connection failures | Fix connection strings / SQL access; ensure same DB as Identity seed. |

## Final URLs (local template)

| Role | URL |
|------|-----|
| **Portal** | http://localhost:3000 |
| **APIs** | http://localhost/hms, /lms, /lis, /pharmacy, /shared, /identity, /communication |

Example health: `http://localhost/hms/health`, `http://localhost/identity/health`.
