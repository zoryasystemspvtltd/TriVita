namespace LISService.Application.DTOs.Analyzer;

/// <summary>Deserialized LMS barcode resolution JSON (same contract as LmsBarcodeResolutionDto).</summary>
public sealed class LmsBarcodeResolutionClientDto
{
    public long FacilityId { get; set; }
    public long TestBookingItemId { get; set; }
    public long LabTestBookingId { get; set; }
    public long CatalogTestId { get; set; }
    public string TestCode { get; set; } = null!;
    public string TestName { get; set; } = null!;
    public long PatientId { get; set; }
    public long? VisitId { get; set; }
    public string BarcodeValue { get; set; } = null!;
    public List<LmsEquipmentAssayClientDto> EquipmentAssays { get; set; } = new();
    public List<LmsCatalogParameterClientDto> Parameters { get; set; } = new();
}

public sealed class LmsEquipmentAssayClientDto
{
    public long EquipmentId { get; set; }
    public string EquipmentAssayCode { get; set; } = null!;
    public string? EquipmentAssayName { get; set; }
}

public sealed class LmsCatalogParameterClientDto
{
    public long CatalogParameterId { get; set; }
    public string ParameterCode { get; set; } = null!;
    public string ParameterName { get; set; } = null!;
    public bool IsNumeric { get; set; }
    public long? UnitId { get; set; }
}

public sealed class AnalyzerQueryTestResponseDto
{
    public string BarcodeValue { get; set; } = null!;
    public long LmsCatalogTestId { get; set; }
    public string TestCode { get; set; } = null!;
    public IReadOnlyList<string> EquipmentTestCodes { get; set; } = Array.Empty<string>();
    public IReadOnlyList<AnalyzerEquipmentAssayDto> EquipmentAssays { get; set; } = Array.Empty<AnalyzerEquipmentAssayDto>();
}

public sealed class AnalyzerEquipmentAssayDto
{
    public long EquipmentId { get; set; }
    public string EquipmentAssayCode { get; set; } = null!;
    public string? EquipmentAssayName { get; set; }
}

public sealed class AnalyzerResultIngestDto
{
    public string Barcode { get; set; } = null!;
    public string EquipmentTestCode { get; set; } = null!;
    public long? EquipmentId { get; set; }
    public long ResultHeaderStatusReferenceValueId { get; set; }
    public long ResultLineStatusReferenceValueId { get; set; }
    public bool TechnicallyVerified { get; set; }
    public bool ReadyForDispatch { get; set; }
    public string? PatientEmail { get; set; }
    public List<AnalyzerResultValueDto> Values { get; set; } = new();
}

public sealed class AnalyzerResultValueDto
{
    public string? EquipmentResultCode { get; set; }
    public long? LmsCatalogParameterId { get; set; }
    public decimal? Numeric { get; set; }
    public string? Text { get; set; }
    public long? UnitId { get; set; }
}

public sealed class AnalyzerResultIngestResponseDto
{
    public long AnalyzerResultHeaderId { get; set; }
}
