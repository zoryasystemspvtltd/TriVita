namespace LISService.Application.DTOs.Entities;

#region LisTestParameterProfile
public sealed class TestParameterProfileResponseDto
{
    public long Id { get; set; }
    public long? FacilityId { get; set; }
    public long TestParameterId { get; set; }
    public long? MethodReferenceValueId { get; set; }
    public long? CollectionMethodReferenceValueId { get; set; }
    public long? ContainerTypeReferenceValueId { get; set; }
    public string? AnalyzerChannelCode { get; set; }
    public string? LoincCode { get; set; }
    public string? Notes { get; set; }
}

public sealed class CreateTestParameterProfileDto
{
    public long TestParameterId { get; set; }
    public long? MethodReferenceValueId { get; set; }
    public long? CollectionMethodReferenceValueId { get; set; }
    public long? ContainerTypeReferenceValueId { get; set; }
    public string? AnalyzerChannelCode { get; set; }
    public string? LoincCode { get; set; }
    public string? Notes { get; set; }
}

public sealed class UpdateTestParameterProfileDto
{
    public long TestParameterId { get; set; }
    public long? MethodReferenceValueId { get; set; }
    public long? CollectionMethodReferenceValueId { get; set; }
    public long? ContainerTypeReferenceValueId { get; set; }
    public string? AnalyzerChannelCode { get; set; }
    public string? LoincCode { get; set; }
    public string? Notes { get; set; }
}
#endregion

#region LisAnalyzerResultMap
public sealed class AnalyzerResultMapResponseDto
{
    public long Id { get; set; }
    public long FacilityId { get; set; }
    public long EquipmentId { get; set; }
    public string ExternalTestCode { get; set; } = null!;
    public string ExternalParameterCode { get; set; } = string.Empty;
    public long TestMasterId { get; set; }
    public long? TestParameterId { get; set; }
    public long? ProtocolReferenceValueId { get; set; }
    public long? UnitOverrideId { get; set; }
}

public sealed class CreateAnalyzerResultMapDto
{
    public long EquipmentId { get; set; }
    public string ExternalTestCode { get; set; } = null!;
    public string? ExternalParameterCode { get; set; }
    public long TestMasterId { get; set; }
    public long? TestParameterId { get; set; }
    public long? ProtocolReferenceValueId { get; set; }
    public long? UnitOverrideId { get; set; }
}

public sealed class UpdateAnalyzerResultMapDto
{
    public long EquipmentId { get; set; }
    public string ExternalTestCode { get; set; } = null!;
    public string? ExternalParameterCode { get; set; }
    public long TestMasterId { get; set; }
    public long? TestParameterId { get; set; }
    public long? ProtocolReferenceValueId { get; set; }
    public long? UnitOverrideId { get; set; }
}
#endregion

#region LisSampleBarcode
public sealed class SampleBarcodeResponseDto
{
    public long Id { get; set; }
    public long FacilityId { get; set; }
    public long SampleCollectionId { get; set; }
    public string BarcodeValue { get; set; } = null!;
    public string? QrPayload { get; set; }
    public long? IdentifierTypeReferenceValueId { get; set; }
    public DateTime? PrintedOn { get; set; }
}

public sealed class CreateSampleBarcodeDto
{
    public long SampleCollectionId { get; set; }
    public string BarcodeValue { get; set; } = null!;
    public string? QrPayload { get; set; }
    public long? IdentifierTypeReferenceValueId { get; set; }
    public DateTime? PrintedOn { get; set; }
}

public sealed class UpdateSampleBarcodeDto
{
    public long SampleCollectionId { get; set; }
    public string BarcodeValue { get; set; } = null!;
    public string? QrPayload { get; set; }
    public long? IdentifierTypeReferenceValueId { get; set; }
    public DateTime? PrintedOn { get; set; }
}
#endregion

#region LisSampleLifecycleEvent
public sealed class SampleLifecycleEventResponseDto
{
    public long Id { get; set; }
    public long FacilityId { get; set; }
    public long SampleCollectionId { get; set; }
    public long? LabOrderItemId { get; set; }
    public long EventTypeReferenceValueId { get; set; }
    public DateTime EventOn { get; set; }
    public DateTime? PlannedDueOn { get; set; }
    public bool TatBreached { get; set; }
    public long? LocationDepartmentId { get; set; }
    public string? EventNotes { get; set; }
}

public sealed class CreateSampleLifecycleEventDto
{
    public long SampleCollectionId { get; set; }
    public long? LabOrderItemId { get; set; }
    public long EventTypeReferenceValueId { get; set; }
    public DateTime EventOn { get; set; }
    public DateTime? PlannedDueOn { get; set; }
    public bool TatBreached { get; set; }
    public long? LocationDepartmentId { get; set; }
    public string? EventNotes { get; set; }
}

public sealed class UpdateSampleLifecycleEventDto
{
    public long SampleCollectionId { get; set; }
    public long? LabOrderItemId { get; set; }
    public long EventTypeReferenceValueId { get; set; }
    public DateTime EventOn { get; set; }
    public DateTime? PlannedDueOn { get; set; }
    public bool TatBreached { get; set; }
    public long? LocationDepartmentId { get; set; }
    public string? EventNotes { get; set; }
}
#endregion

#region LisReportDeliveryOtp
public sealed class ReportDeliveryOtpResponseDto
{
    public long Id { get; set; }
    public long FacilityId { get; set; }
    public long ReportHeaderId { get; set; }
    public string OtpHash { get; set; } = null!;
    public DateTime ExpiresOn { get; set; }
    public DateTime? ConsumedOn { get; set; }
    public long? DeliveryChannelReferenceValueId { get; set; }
}

public sealed class CreateReportDeliveryOtpDto
{
    public long ReportHeaderId { get; set; }
    public string OtpHash { get; set; } = null!;
    public DateTime ExpiresOn { get; set; }
    public long? DeliveryChannelReferenceValueId { get; set; }
}

public sealed class UpdateReportDeliveryOtpDto
{
    public long ReportHeaderId { get; set; }
    public string OtpHash { get; set; } = null!;
    public DateTime ExpiresOn { get; set; }
    public DateTime? ConsumedOn { get; set; }
    public long? DeliveryChannelReferenceValueId { get; set; }
}
#endregion

#region LisReportLockState
public sealed class ReportLockStateResponseDto
{
    public long Id { get; set; }
    public long FacilityId { get; set; }
    public long ReportHeaderId { get; set; }
    public bool IsLocked { get; set; }
    public DateTime? LockedOn { get; set; }
    public long? LockedByUserId { get; set; }
    public long? LockReasonReferenceValueId { get; set; }
}

public sealed class CreateReportLockStateDto
{
    public long ReportHeaderId { get; set; }
    public bool IsLocked { get; set; }
    public DateTime? LockedOn { get; set; }
    public long? LockedByUserId { get; set; }
    public long? LockReasonReferenceValueId { get; set; }
}

public sealed class UpdateReportLockStateDto
{
    public long ReportHeaderId { get; set; }
    public bool IsLocked { get; set; }
    public DateTime? LockedOn { get; set; }
    public long? LockedByUserId { get; set; }
    public long? LockReasonReferenceValueId { get; set; }
}
#endregion
