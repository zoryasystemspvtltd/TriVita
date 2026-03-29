namespace SharedService.Application.DTOs.Enterprise;

public sealed class EnterpriseHierarchyResponseDto
{
    public EnterpriseResponseDto Enterprise { get; init; } = null!;

    public IReadOnlyList<CompanyHierarchyNodeDto> Companies { get; init; } = Array.Empty<CompanyHierarchyNodeDto>();
}

public sealed class CompanyHierarchyNodeDto
{
    public CompanyResponseDto Company { get; init; } = null!;

    public IReadOnlyList<BusinessUnitHierarchyNodeDto> BusinessUnits { get; init; } = Array.Empty<BusinessUnitHierarchyNodeDto>();
}

public sealed class BusinessUnitHierarchyNodeDto
{
    public BusinessUnitResponseDto BusinessUnit { get; init; } = null!;

    public IReadOnlyList<FacilityHierarchyNodeDto> Facilities { get; init; } = Array.Empty<FacilityHierarchyNodeDto>();
}

public sealed class FacilityHierarchyNodeDto
{
    public FacilityResponseDto Facility { get; init; } = null!;

    public IReadOnlyList<DepartmentResponseDto> Departments { get; init; } = Array.Empty<DepartmentResponseDto>();
}
