using Healthcare.Common.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TriVita.UnifiedDatabase;

/// <summary>
/// Design-time factory for <c>dotnet ef</c>.
/// Set <c>TRIVITA_UNIFIED_SQL</c> for the connection string.
/// Set <c>TRIVITA_USE_MODULE_SCHEMAS=false</c> to keep tables in <c>dbo</c> (legacy script alignment).
/// </summary>
public sealed class HealthcareDbContextFactory : IDesignTimeDbContextFactory<HealthcareDbContext>
{
    public HealthcareDbContext CreateDbContext(string[] args)
    {
        var cs = Environment.GetEnvironmentVariable("TRIVITA_UNIFIED_SQL")
                 ?? "Server=.\\SQLEXPRESS;Database=TriVitaHealthcare;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true";

        var useModuleSchemas = !string.Equals(
            Environment.GetEnvironmentVariable("TRIVITA_USE_MODULE_SCHEMAS"),
            "false",
            StringComparison.OrdinalIgnoreCase);

        var options = new DbContextOptionsBuilder<HealthcareDbContext>()
            .UseSqlServer(cs, sql => sql.MigrationsAssembly(typeof(HealthcareDbContext).Assembly.GetName().Name ?? "TriVita.UnifiedDatabase"))
            .Options;

        ITenantContext tenant = new MigrationTenantContext();
        var modelOpts = new TriVitaUnifiedModelOptions { UseModuleSchemas = useModuleSchemas };

        return new HealthcareDbContext(options, tenant, modelOpts);
    }
}
