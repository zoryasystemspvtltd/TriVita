namespace Healthcare.Common.Integration.SharedService;

/// <summary>Validates that a facility belongs to the tenant via SharedService enterprise hierarchy.</summary>
public interface IFacilityTenantValidator
{
    /// <summary>Returns hierarchy context when the facility exists for the tenant; otherwise null.</summary>
    Task<FacilityHierarchyContext?> GetFacilityContextAsync(
        long tenantId,
        long facilityId,
        CancellationToken cancellationToken = default);
}
