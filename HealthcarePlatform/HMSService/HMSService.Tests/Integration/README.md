# Optional integration tests

Template for end-to-end API tests:

1. Add package `Microsoft.AspNetCore.Mvc.Testing` to this project (or a dedicated `*.IntegrationTests` project).
2. Use `WebApplicationFactory<Program>` — ensure `HMSService.API` exposes `public partial class Program { }` if required by your SDK version.
3. Replace SQL Server with `UseInMemoryDatabase` in test configuration **or** use Docker SQL for full fidelity.
4. Send `Authorization: Bearer <token>` and `X-Tenant-Id` headers to satisfy `RequireTenantContextMiddleware`.

This folder is intentionally empty so **unit** tests stay fast and CI-friendly.
