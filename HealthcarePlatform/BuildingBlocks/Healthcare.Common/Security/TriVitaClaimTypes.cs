namespace Healthcare.Common.Security;

/// <summary>Custom JWT / claims used across TriVita services.</summary>
public static class TriVitaClaimTypes
{
    public const string TenantId = "tenant_id";

    public const string FacilityId = "facility_id";

    /// <summary>Additional facilities the user may act within (multi-value).</summary>
    public const string AllowedFacility = "allowed_facility";

    /// <summary>Granular permission codes (multi-value), e.g. hms.api.</summary>
    public const string Permission = "permission";
}
