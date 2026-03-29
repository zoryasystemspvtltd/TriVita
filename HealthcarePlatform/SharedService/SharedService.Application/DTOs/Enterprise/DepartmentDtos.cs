namespace SharedService.Application.DTOs.Enterprise;

public sealed class DepartmentResponseDto
{
    public long Id { get; init; }

    public long FacilityId { get; init; }

    public long FacilityParentId { get; init; }

    public string DepartmentCode { get; init; } = null!;

    public string DepartmentName { get; init; } = null!;

    public string DepartmentType { get; init; } = null!;

    public long? ParentDepartmentId { get; init; }

    public long? PrimaryAddressId { get; init; }

    public long? PrimaryContactId { get; init; }

    public DateTime? EffectiveFrom { get; init; }

    public DateTime? EffectiveTo { get; init; }

    public bool IsActive { get; init; }
}

public sealed class CreateDepartmentDto
{
    public long FacilityParentId { get; init; }

    public string DepartmentCode { get; init; } = null!;

    public string DepartmentName { get; init; } = null!;

    public string DepartmentType { get; init; } = null!;

    public long? ParentDepartmentId { get; init; }

    public long? PrimaryAddressId { get; init; }

    public long? PrimaryContactId { get; init; }

    public DateTime? EffectiveFrom { get; init; }

    public DateTime? EffectiveTo { get; init; }
}

public sealed class UpdateDepartmentDto
{
    public long FacilityParentId { get; init; }

    public string DepartmentCode { get; init; } = null!;

    public string DepartmentName { get; init; } = null!;

    public string DepartmentType { get; init; } = null!;

    public long? ParentDepartmentId { get; init; }

    public long? PrimaryAddressId { get; init; }

    public long? PrimaryContactId { get; init; }

    public DateTime? EffectiveFrom { get; init; }

    public DateTime? EffectiveTo { get; init; }

    public bool IsActive { get; init; } = true;
}
