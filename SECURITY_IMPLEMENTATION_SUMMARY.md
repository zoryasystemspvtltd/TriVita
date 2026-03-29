# TriVita — Security & identity implementation summary

This document summarizes the incremental identity/RBAC work delivered in code and SQL. It is intended for architects and operators wiring environments together.

## 1. Database: `08_Identity_Enhancement.sql`

- **Location:** repository root, `08_Identity_Enhancement.sql`.
- **Rules:** No `ALTER` of existing 00–07 tables. Only new tables (and conditional FKs where prerequisite tables exist).
- **Identity_* (JWT / IdentityService store):** user profile extension, roles, permissions, user–role and role–permission links, facility grants, refresh tokens, lockout state, login attempts, password reset tokens, user activity log, MFA pending challenge (structure), with `TenantId` / `FacilityId` and standard audit columns where applicable.
- **IAM_* extensions (enterprise IAM from script 07):** refresh tokens, user security state (lockout counters), role hierarchy, user permission grants (including optional resource scope), login audit, authorization policy definitions, role–policy bindings, role–business-unit scope.

**Deployment note:** `Identity_*` tables that FK to `Identity_Users` are created only when `dbo.Identity_Users` already exists (typically after IdentityService EF `EnsureCreated` or your migration pipeline).

## 2. IdentityService (API + persistence)

- **Authentication:** `POST /api/v1/auth/login` and `POST /api/v1/auth/token`, `POST /auth/refresh-token`, `POST /auth/logout`, `POST /auth/forgot-password`, `POST /auth/reset-password`, `GET /auth/me`.
- **JWT claims:** `sub` (user id), `email`, `tenant_id`, `facility_id`, multiple `role` claims, multiple `permission` claims, optional `allowed_facility` for extra facilities.
- **Refresh tokens:** Opaque URL-safe token stored as SHA-256 hash; rotation on refresh; revocation on logout.
- **Lockout & audit:** Failed password/MFA attempts increment lockout (configurable thresholds); login attempts written to `Identity_LoginAttempt`.
- **MFA:** `Identity_UserProfile.MfaEnabled` + optional `Security:MfaDevBypassCode` for controlled dev verification (replace with real TOTP/WebAuthn in production).
- **Admin APIs:** `POST /api/v1/identity-admin/*` guarded by permission `identity.admin` — create user, assign roles/facilities, create role/permission, assign permissions to role.
- **Configuration:** `Security` section in `appsettings.json` (lockout, token lifetimes, MFA bypass for dev).
- **Swagger:** Identity API always registers `UseSwagger` + `UseTriVitaSwaggerUi` so JWT bearer can be exercised without extra flags.

## 3. Cross-cutting RBAC (Healthcare.Common)

- **Claim types:** `TriVitaClaimTypes` (`tenant_id`, `facility_id`, `allowed_facility`, `permission`).
- **Permission codes:** `TriVitaPermissions` (`hms.api`, `lis.api`, `lms.api`, `pharmacy.api`, `shared.api`, `communication.api`, `identity.admin`, `*`).
- **Dynamic policies:** `Permission:{code}` resolved by `PermissionPolicyProvider`; `PermissionAuthorizationHandler` allows access if the user has the permission claim, the `*` permission, or the **Admin** role.
- **Tenant / facility alignment:** `SecurityContextAlignmentMiddleware` rejects mismatched `X-Tenant-Id` vs token and rejects `X-Facility-Id` outside primary `facility_id` or `allowed_facility` claims (Admin / `*` exempt).
- **Test auth:** `TestAuthenticationHandler` issues `Admin` + `*` permission so integration tests keep working when `IntegrationTest:UseTestAuth=true`.

## 4. Microservices wired for RBAC

For **HMSService, LISService, LMSService, PharmacyService, SharedService, CommunicationService**:

- `AddTriVitaPermissionAuthorization()` after `AddAuthorization()`.
- `UseTriVitaSecurityContextAlignment()` after `UseAuthentication()`.
- Controller-level `[Authorize]` replaced with `[RequirePermission(TriVitaPermissions.<ServiceApi>)]` (one coarse permission per service; refine per controller/action as the permission matrix matures).

## 5. Seeding (development)

`IdentityDataSeeder` seeds tenant `1` with all standard permissions, an **Admin** role wired to those permissions, demo user `admin@demo.local` / `ChangeMe!123`, profile, facility grant `1`, and user–role assignment.

## 6. Tests: `IdentityService.Tests`

- **xUnit + Moq + FluentAssertions** (see project under `IdentityService/IdentityService.Tests`).
- Covers sample **AuthService** flows, **PermissionAuthorizationHandler** behavior, and **AuthController** delegation.
- **Coverage:** The solution does not yet enforce the stated ≥90% / ≥80% line targets; expand tests where compliance is required.

## 7. Operational hardening checklist (next steps)

- Remove `MfaDevBypassCode` outside development; implement TOTP/WebAuthn using `Identity_MfaPendingChallenge` / IAM MFA tables as appropriate.
- Rotate JWT signing keys via configuration/secrets store; shorten access token lifetime if refresh is fully trusted.
- Enable refresh token rotation abuse detection (reuse of revoked tokens → revoke family) if threat model requires it.
- Align **LMSService** IAM APIs (script 07) with this identity model to avoid two competing writers to IAM (pick a single source of truth or sync strategy).
- Run `08_Identity_Enhancement.sql` on shared SQL Server databases and align EF migrations if you move off `EnsureCreated`.
