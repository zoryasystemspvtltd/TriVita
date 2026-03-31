using Healthcare.Common.MultiTenancy;

namespace TriVita.UnifiedDatabase;

/// <summary>Fixed tenant scope for design-time migrations (dotnet ef). Not used by microservices at runtime.</summary>
public sealed class MigrationTenantContext : ITenantContext
{
    public long TenantId => 1;

    public long? FacilityId => 1;

    public long? UserId => 0;

    public IReadOnlyList<string> Roles { get; } = Array.Empty<string>();

    public IReadOnlyList<string> Permissions { get; } = Array.Empty<string>();
}
