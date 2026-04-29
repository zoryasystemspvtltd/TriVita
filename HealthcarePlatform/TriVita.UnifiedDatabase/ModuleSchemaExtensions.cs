using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TriVita.UnifiedDatabase;

public static class ModuleSchemaExtensions
{
    /// <summary>Maps CLR namespaces to SQL schemas for single-database modular layout.</summary>
    public static void ApplyModuleSchemas(this ModelBuilder modelBuilder, bool enabled)
    {
        if (!enabled) return;

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (entityType.IsOwned()) continue;

            var clr = entityType.ClrType;
            // Supplier and Form masters live in dbo (shared-style); do not assign pharmacy schema.
            if (string.Equals(clr.FullName, "PharmacyService.Domain.Entities.PhrSupplier", StringComparison.Ordinal)
                || string.Equals(clr.FullName, "PharmacyService.Domain.Entities.PhrForm", StringComparison.Ordinal))
                continue;

            var ns = clr.Namespace ?? string.Empty;
            var schema = ResolveSchema(ns);
            if (schema is null) continue;

            if (entityType is IMutableEntityType mutable)
                mutable.SetSchema(schema);
        }
    }

    private static string? ResolveSchema(string ns)
    {
        if (ns.StartsWith("HMSService.Domain", StringComparison.Ordinal)) return "hms";
        if (ns.StartsWith("LMSService.Domain", StringComparison.Ordinal)) return "lms";
        if (ns.StartsWith("LISService.Domain", StringComparison.Ordinal)) return "lis";
        if (ns.StartsWith("PharmacyService.Domain", StringComparison.Ordinal)) return "pharmacy";
        if (ns.StartsWith("SharedService.Domain", StringComparison.Ordinal)) return "shared";
        if (ns.StartsWith("IdentityService.Domain", StringComparison.Ordinal)) return "identity";
        if (ns.StartsWith("CommunicationService.Domain", StringComparison.Ordinal)) return "communication";
        return null;
    }
}
