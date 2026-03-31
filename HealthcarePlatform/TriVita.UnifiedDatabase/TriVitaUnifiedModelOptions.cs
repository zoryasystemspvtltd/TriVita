namespace TriVita.UnifiedDatabase;

/// <summary>Model-wide options for the unified EF Core context (migrations + optional runtime use).</summary>
public sealed class TriVitaUnifiedModelOptions
{
    /// <summary>
    /// When true, assigns SQL schemas (hms, lms, lis, pharmacy, shared, identity, communication) from entity CLR namespaces.
    /// When false, tables remain in default schema (typically dbo) — use for alignment with legacy script-created databases.
    /// </summary>
    public bool UseModuleSchemas { get; init; } = true;
}
