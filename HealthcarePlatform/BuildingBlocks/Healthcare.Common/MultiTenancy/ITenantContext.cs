namespace Healthcare.Common.MultiTenancy;

/// <summary>
/// Resolved per-request from JWT claims and/or headers (X-Tenant-Id, X-Facility-Id).
/// </summary>
public interface ITenantContext
{
    long TenantId { get; }

    long? FacilityId { get; }

    long? UserId { get; }

    IReadOnlyList<string> Roles { get; }

    IReadOnlyList<string> Permissions { get; }
}
