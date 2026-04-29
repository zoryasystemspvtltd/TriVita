using Healthcare.Common.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace TriVita.UnifiedDatabase;

/// <summary>
/// Design-time factory for <c>dotnet ef</c>.
/// Connection: <c>ConnectionStrings__DefaultConnection</c> / <c>DefaultConnection</c> env vars, then <c>appsettings.json</c> in output, else <c>.\SQLEXPRESS</c> + <c>TriVita</c>.
/// Set <c>TRIVITA_USE_MODULE_SCHEMAS=false</c> to keep tables in <c>dbo</c> (legacy script alignment).
/// </summary>
public sealed class HealthcareDbContextFactory : IDesignTimeDbContextFactory<HealthcareDbContext>
{
    internal const string TriVitaSqlExpressConnection =
        "Server=.\\SQLEXPRESS;Database=TriVita;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true";

    public HealthcareDbContext CreateDbContext(string[] args)
    {
        var cs = ResolveConnectionString();
        if (string.IsNullOrWhiteSpace(cs) || !cs.Contains("Database=TriVita", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Connection string must use database TriVita (DefaultConnection).");

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

    private static string ResolveConnectionString()
    {
        var env =
            Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? Environment.GetEnvironmentVariable("DefaultConnection");
        if (!string.IsNullOrWhiteSpace(env))
        {
            ThrowIfLocalDb(env);
            return env;
        }

        var baseDir = AppContext.BaseDirectory;
        var path = Path.Combine(baseDir, "appsettings.json");
        if (File.Exists(path))
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(baseDir)
                .AddJsonFile("appsettings.json", optional: false)
                .AddEnvironmentVariables()
                .Build();
            var fromConfig = config.GetConnectionString("DefaultConnection");
            if (!string.IsNullOrWhiteSpace(fromConfig))
            {
                ThrowIfLocalDb(fromConfig);
                return fromConfig;
            }
        }

        ThrowIfLocalDb(TriVitaSqlExpressConnection);
        return TriVitaSqlExpressConnection;
    }

    private static void ThrowIfLocalDb(string connectionString)
    {
        if (connectionString.Contains("(localdb)", StringComparison.OrdinalIgnoreCase)
            || connectionString.Contains("mssqllocaldb", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("LocalDB is not allowed. Use Server=.\\SQLEXPRESS;Database=TriVita;...");
    }
}
