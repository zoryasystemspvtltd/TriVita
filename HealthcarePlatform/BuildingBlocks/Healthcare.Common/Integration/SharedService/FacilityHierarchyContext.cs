namespace Healthcare.Common.Integration.SharedService;

/// <summary>Enterprise chain for a facility; aligns with SharedService <c>FacilityHierarchyContextDto</c> JSON.</summary>
public sealed class FacilityHierarchyContext
{
    public long TenantId { get; set; }

    public long EnterpriseId { get; set; }

    public long CompanyId { get; set; }

    public long BusinessUnitId { get; set; }

    public long FacilityId { get; set; }

    public string FacilityCode { get; set; } = "";

    public string FacilityName { get; set; } = "";

    public string BusinessUnitType { get; set; } = "";

    public string FacilityType { get; set; } = "";
}
