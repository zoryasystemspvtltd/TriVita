# Repository tests

`EfRepository` / SQL-backed repositories are not exercised in **unit** tests here (they depend on EF Core and tenant-scoped `DbContext`).

**Options:**

- **Integration tests** — `WebApplicationFactory` + in-memory or Testcontainers SQL Server (see `../Integration/`).
- **In-memory EF** — register `UseInMemoryDatabase` in a test `DbContext` and test query logic in isolation.
