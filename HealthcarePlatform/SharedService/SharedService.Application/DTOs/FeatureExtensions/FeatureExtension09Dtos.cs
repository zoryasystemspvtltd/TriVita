namespace SharedService.Application.DTOs.FeatureExtensions;

#region Facility service price list

public sealed class FacilityServicePriceListResponseDto
{
    public long Id { get; init; }
    public long FacilityId { get; init; }
    public string PriceListCode { get; init; } = null!;
    public string PriceListName { get; init; } = null!;
    public string ServiceModule { get; init; } = null!;
    public string? PartnerReferenceCode { get; init; }
    public string CurrencyCode { get; init; } = null!;
    public DateTime? EffectiveFrom { get; init; }
    public DateTime? EffectiveTo { get; init; }
    public bool IsActive { get; init; }
}

public sealed class CreateFacilityServicePriceListDto
{
    public long FacilityId { get; init; }
    public string PriceListCode { get; init; } = null!;
    public string PriceListName { get; init; } = null!;
    public string ServiceModule { get; init; } = null!;
    public string? PartnerReferenceCode { get; init; }
    public string CurrencyCode { get; init; } = "INR";
    public DateTime? EffectiveFrom { get; init; }
    public DateTime? EffectiveTo { get; init; }
}

public sealed class UpdateFacilityServicePriceListDto
{
    public string PriceListName { get; init; } = null!;
    public string ServiceModule { get; init; } = null!;
    public string? PartnerReferenceCode { get; init; }
    public string CurrencyCode { get; init; } = null!;
    public DateTime? EffectiveFrom { get; init; }
    public DateTime? EffectiveTo { get; init; }
    public bool IsActive { get; init; } = true;
}

#endregion

#region Facility service price list line

public sealed class FacilityServicePriceListLineResponseDto
{
    public long Id { get; init; }
    public long FacilityId { get; init; }
    public long PriceListId { get; init; }
    public string ServiceItemCode { get; init; } = null!;
    public string? ServiceItemName { get; init; }
    public decimal UnitPrice { get; init; }
    public string? TaxCategoryCode { get; init; }
    public bool IsActive { get; init; }
}

public sealed class CreateFacilityServicePriceListLineDto
{
    public long FacilityId { get; init; }
    public long PriceListId { get; init; }
    public string ServiceItemCode { get; init; } = null!;
    public string? ServiceItemName { get; init; }
    public decimal UnitPrice { get; init; }
    public string? TaxCategoryCode { get; init; }
}

public sealed class UpdateFacilityServicePriceListLineDto
{
    public string? ServiceItemName { get; init; }
    public decimal UnitPrice { get; init; }
    public string? TaxCategoryCode { get; init; }
    public bool IsActive { get; init; } = true;
}

#endregion

#region Cross-facility report audit

public sealed class CrossFacilityReportAuditResponseDto
{
    public long Id { get; init; }
    public long? FacilityId { get; init; }
    public string ReportCode { get; init; } = null!;
    public string? ReportName { get; init; }
    public string? FacilityScopeJson { get; init; }
    public string? FilterJson { get; init; }
    public int? ResultRowCount { get; init; }
    public DateTime? CompletedOn { get; init; }
    public bool IsActive { get; init; }
}

public sealed class CreateCrossFacilityReportAuditDto
{
    public long? FacilityId { get; init; }
    public string ReportCode { get; init; } = null!;
    public string? ReportName { get; init; }
    public string? FacilityScopeJson { get; init; }
    public string? FilterJson { get; init; }
}

public sealed class UpdateCrossFacilityReportAuditDto
{
    public string? ReportName { get; init; }
    public int? ResultRowCount { get; init; }
    public DateTime? CompletedOn { get; init; }
    public bool IsActive { get; init; } = true;
}

#endregion

#region Module integration handoff

public sealed class ModuleIntegrationHandoffResponseDto
{
    public long Id { get; init; }
    public long? FacilityId { get; init; }
    public string CorrelationId { get; init; } = null!;
    public string SourceModule { get; init; } = null!;
    public string TargetModule { get; init; } = null!;
    public string EntityType { get; init; } = null!;
    public long? SourceEntityId { get; init; }
    public long? TargetEntityId { get; init; }
    public string StatusCode { get; init; } = null!;
    public string? DetailJson { get; init; }
    public bool IsActive { get; init; }
}

public sealed class CreateModuleIntegrationHandoffDto
{
    public long? FacilityId { get; init; }
    public string CorrelationId { get; init; } = null!;
    public string SourceModule { get; init; } = null!;
    public string TargetModule { get; init; } = null!;
    public string EntityType { get; init; } = null!;
    public long? SourceEntityId { get; init; }
    public string StatusCode { get; init; } = null!;
    public string? DetailJson { get; init; }
}

public sealed class UpdateModuleIntegrationHandoffDto
{
    public long? TargetEntityId { get; init; }
    public string StatusCode { get; init; } = null!;
    public string? DetailJson { get; init; }
    public bool IsActive { get; init; } = true;
}

#endregion

#region Tenant onboarding stage

public sealed class TenantOnboardingStageResponseDto
{
    public long Id { get; init; }
    public long? FacilityId { get; init; }
    public string StageCode { get; init; } = null!;
    public string StageStatus { get; init; } = null!;
    public DateTime? CompletedOn { get; init; }
    public string? MetadataJson { get; init; }
    public bool IsActive { get; init; }
}

public sealed class UpsertTenantOnboardingStageDto
{
    public long? FacilityId { get; init; }
    public string StageCode { get; init; } = null!;
    public string StageStatus { get; init; } = null!;
    public DateTime? CompletedOn { get; init; }
    public string? MetadataJson { get; init; }
    public bool IsActive { get; init; } = true;
}

#endregion

#region Lab critical value escalation

public sealed class LabCriticalValueEscalationResponseDto
{
    public long Id { get; init; }
    public long FacilityId { get; init; }
    public long? LabOrderId { get; init; }
    public long? LabOrderItemId { get; init; }
    public long? LabResultId { get; init; }
    public int EscalationLevel { get; init; }
    public string ChannelCode { get; init; } = null!;
    public string? RecipientSummary { get; init; }
    public DateTime? DispatchedOn { get; init; }
    public DateTime? AcknowledgedOn { get; init; }
    public string? OutcomeCode { get; init; }
    public bool IsActive { get; init; }
}

public sealed class CreateLabCriticalValueEscalationDto
{
    public long FacilityId { get; init; }
    public long? LabOrderId { get; init; }
    public long? LabOrderItemId { get; init; }
    public long? LabResultId { get; init; }
    public int EscalationLevel { get; init; } = 1;
    public string ChannelCode { get; init; } = null!;
    public string? RecipientSummary { get; init; }
    public DateTime? DispatchedOn { get; init; }
}

public sealed class UpdateLabCriticalValueEscalationDto
{
    public DateTime? DispatchedOn { get; init; }
    public DateTime? AcknowledgedOn { get; init; }
    public string? OutcomeCode { get; init; }
    public bool IsActive { get; init; } = true;
}

#endregion
