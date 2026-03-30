using Healthcare.Common.Entities;

namespace LMSService.Domain.Entities;

public sealed class LmsEquipmentType : BaseEntity
{
    public string TypeCode { get; set; } = null!;
    public string TypeName { get; set; } = null!;
    public string? Description { get; set; }
}

public sealed class LmsEquipmentFacilityMapping : BaseEntity
{
    public long EquipmentFacilityId { get; set; }
    public long EquipmentId { get; set; }
    public long MappedFacilityId { get; set; }
    public string? MappingNotes { get; set; }
}

public sealed class LmsCatalogTest : BaseEntity
{
    public string TestCode { get; set; } = null!;
    public string TestName { get; set; } = null!;
    public string? TestDescription { get; set; }
    public long DisciplineReferenceValueId { get; set; }
    public long? SampleTypeReferenceValueId { get; set; }
    public bool IsRadiology { get; set; }
    public long? DefaultUnitId { get; set; }
}

public sealed class LmsCatalogParameter : BaseEntity
{
    public string ParameterCode { get; set; } = null!;
    public string ParameterName { get; set; } = null!;
    public bool IsNumeric { get; set; } = true;
    public long? UnitId { get; set; }
    public string? ParameterNotes { get; set; }
}

public sealed class LmsCatalogReferenceRange : BaseEntity
{
    public long CatalogParameterId { get; set; }
    public long? SexReferenceValueId { get; set; }
    public int? AgeFromYears { get; set; }
    public int? AgeToYears { get; set; }
    public decimal? MinValue { get; set; }
    public decimal? MaxValue { get; set; }
    public string? RangeText { get; set; }
    public string? RangeNotes { get; set; }
}

public sealed class LmsCatalogTestParameterMap : BaseEntity
{
    public long CatalogTestId { get; set; }
    public long CatalogParameterId { get; set; }
    public int DisplayOrder { get; set; }
}

public sealed class LmsCatalogPackageParameterMap : BaseEntity
{
    public long TestPackageId { get; set; }
    public long CatalogParameterId { get; set; }
    public long? CatalogTestId { get; set; }
}

public sealed class LmsEquipmentTestMaster : BaseEntity
{
    public long EquipmentId { get; set; }
    public long CatalogTestId { get; set; }
    public string EquipmentAssayCode { get; set; } = null!;
    public string? EquipmentAssayName { get; set; }
}

public sealed class LmsCatalogTestEquipmentMap : BaseEntity
{
    public long CatalogTestId { get; set; }
    public long EquipmentId { get; set; }
    public bool IsPreferred { get; set; }
}

public sealed class LmsCatalogPackageTestLineMap : BaseEntity
{
    public long TestPackageId { get; set; }
    public int LineNum { get; set; }
    public long CatalogTestId { get; set; }
}

public sealed class LmsLabTestBooking : BaseEntity
{
    public string BookingNo { get; set; } = null!;
    public long PatientId { get; set; }
    public long? VisitId { get; set; }
    public long? SourceReferenceValueId { get; set; }
    public string? BookingNotes { get; set; }
}

public sealed class LmsLabTestBookingItem : BaseEntity
{
    public long LabTestBookingId { get; set; }
    public long CatalogTestId { get; set; }
    public long WorkflowStatusReferenceValueId { get; set; }
    public string? LineNotes { get; set; }
}

public sealed class LmsLabSampleBarcode : BaseEntity
{
    public string BarcodeValue { get; set; } = null!;
    public long TestBookingItemId { get; set; }
    public long? SampleTypeReferenceValueId { get; set; }
    public long BarcodeStatusReferenceValueId { get; set; }
    public string? RegisteredFromSystem { get; set; }
}
