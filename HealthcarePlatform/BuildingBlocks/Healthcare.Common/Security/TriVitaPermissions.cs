namespace Healthcare.Common.Security;

/// <summary>Permission codes issued by IdentityService and enforced via RBAC policies.</summary>
public static class TriVitaPermissions
{
    public const string Wildcard = "*";

    public const string HmsApi = "hms.api";

    public const string LisApi = "lis.api";

    public const string LmsApi = "lms.api";

    public const string PharmacyApi = "pharmacy.api";

    public const string SharedApi = "shared.api";

    public const string CommunicationApi = "communication.api";

    public const string IdentityAdmin = "identity.admin";
}
