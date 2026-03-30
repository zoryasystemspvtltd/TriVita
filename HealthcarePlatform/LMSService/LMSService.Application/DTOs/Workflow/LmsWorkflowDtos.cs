namespace LMSService.Application.DTOs.Workflow;

#region Integration (LIS consumer)

public sealed class LmsBarcodeResolutionDto
{
    public long FacilityId { get; init; }
    public long TestBookingItemId { get; init; }
    public long LabTestBookingId { get; init; }
    public long CatalogTestId { get; init; }
    public string TestCode { get; init; } = null!;
    public string TestName { get; init; } = null!;
    public long PatientId { get; init; }
    public long? VisitId { get; init; }
    public string BarcodeValue { get; init; } = null!;
    public IReadOnlyList<LmsEquipmentAssayDto> EquipmentAssays { get; init; } = Array.Empty<LmsEquipmentAssayDto>();
    public IReadOnlyList<LmsCatalogParameterSnapshotDto> Parameters { get; init; } = Array.Empty<LmsCatalogParameterSnapshotDto>();
}

public sealed class LmsEquipmentAssayDto
{
    public long EquipmentId { get; init; }
    public string EquipmentAssayCode { get; init; } = null!;
    public string? EquipmentAssayName { get; init; }
}

public sealed class LmsCatalogParameterSnapshotDto
{
    public long CatalogParameterId { get; init; }
    public string ParameterCode { get; init; } = null!;
    public string ParameterName { get; init; } = null!;
    public bool IsNumeric { get; init; }
    public long? UnitId { get; init; }
}

#endregion

#region Equipment type & facility map

public sealed class LmsEquipmentTypeResponseDto
{
    public long Id { get; init; }
    public long? FacilityId { get; init; }
    public string TypeCode { get; init; } = null!;
    public string TypeName { get; init; } = null!;
    public string? Description { get; init; }
}

public sealed class CreateLmsEquipmentTypeDto
{
    public string TypeCode { get; init; } = null!;
    public string TypeName { get; init; } = null!;
    public string? Description { get; init; }
}

public sealed class UpdateLmsEquipmentTypeDto
{
    public string TypeName { get; init; } = null!;
    public string? Description { get; init; }
    public bool IsActive { get; init; } = true;
}

public sealed class LmsEquipmentFacilityMappingResponseDto
{
    public long Id { get; init; }
    public long FacilityId { get; init; }
    public long EquipmentFacilityId { get; init; }
    public long EquipmentId { get; init; }
    public long MappedFacilityId { get; init; }
    public string? MappingNotes { get; init; }
}

public sealed class CreateLmsEquipmentFacilityMappingDto
{
    public long EquipmentFacilityId { get; init; }
    public long EquipmentId { get; init; }
    public long MappedFacilityId { get; init; }
    public string? MappingNotes { get; init; }
}

public sealed class UpdateLmsEquipmentFacilityMappingDto
{
    public string? MappingNotes { get; init; }
    public bool IsActive { get; init; } = true;
}

#endregion

#region Catalog test (LMS master)

public sealed class LmsCatalogTestResponseDto
{
    public long Id { get; init; }
    public long FacilityId { get; init; }
    public string TestCode { get; init; } = null!;
    public string TestName { get; init; } = null!;
    public string? TestDescription { get; init; }
    public long DisciplineReferenceValueId { get; init; }
    public long? SampleTypeReferenceValueId { get; init; }
    public bool IsRadiology { get; init; }
    public long? DefaultUnitId { get; init; }
}

public sealed class CreateLmsCatalogTestDto
{
    public string TestCode { get; init; } = null!;
    public string TestName { get; init; } = null!;
    public string? TestDescription { get; init; }
    public long DisciplineReferenceValueId { get; init; }
    public long? SampleTypeReferenceValueId { get; init; }
    public bool IsRadiology { get; init; }
    public long? DefaultUnitId { get; init; }
}

public sealed class UpdateLmsCatalogTestDto
{
    public string TestName { get; init; } = null!;
    public string? TestDescription { get; init; }
    public long DisciplineReferenceValueId { get; init; }
    public long? SampleTypeReferenceValueId { get; init; }
    public bool IsRadiology { get; init; }
    public long? DefaultUnitId { get; init; }
    public bool IsActive { get; init; } = true;
}

#endregion

#region Booking & barcode

public sealed class LmsLabTestBookingResponseDto
{
    public long Id { get; init; }
    public long FacilityId { get; init; }
    public string BookingNo { get; init; } = null!;
    public long PatientId { get; init; }
    public long? VisitId { get; init; }
    public long? SourceReferenceValueId { get; init; }
    public IReadOnlyList<LmsLabTestBookingItemResponseDto> Items { get; init; } = Array.Empty<LmsLabTestBookingItemResponseDto>();
}

public sealed class LmsLabTestBookingItemResponseDto
{
    public long Id { get; init; }
    public long CatalogTestId { get; init; }
    public long WorkflowStatusReferenceValueId { get; init; }
    public string? LineNotes { get; init; }
}

public sealed class CreateLmsLabTestBookingItemDto
{
    public long CatalogTestId { get; init; }
    public long WorkflowStatusReferenceValueId { get; init; }
    public string? LineNotes { get; init; }
}

public sealed class CreateLmsLabTestBookingDto
{
    public long PatientId { get; init; }
    public long? VisitId { get; init; }
    public long? SourceReferenceValueId { get; init; }
    public string? BookingNotes { get; init; }
    public IReadOnlyList<CreateLmsLabTestBookingItemDto> Items { get; init; } = Array.Empty<CreateLmsLabTestBookingItemDto>();
}

public sealed class LmsLabSampleBarcodeResponseDto
{
    public long Id { get; init; }
    public long FacilityId { get; init; }
    public string BarcodeValue { get; init; } = null!;
    public long TestBookingItemId { get; init; }
    public long? SampleTypeReferenceValueId { get; init; }
    public long BarcodeStatusReferenceValueId { get; init; }
    public string? RegisteredFromSystem { get; init; }
}

public sealed class RegisterLmsLabSampleBarcodeDto
{
    public string BarcodeValue { get; init; } = null!;
    public long TestBookingItemId { get; init; }
    public long? SampleTypeReferenceValueId { get; init; }
    public long BarcodeStatusReferenceValueId { get; init; }
    public string? RegisteredFromSystem { get; init; }
}

#endregion
