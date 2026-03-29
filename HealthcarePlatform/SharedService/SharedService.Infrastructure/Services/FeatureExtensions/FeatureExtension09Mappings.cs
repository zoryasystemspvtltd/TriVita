using SharedService.Application.DTOs.FeatureExtensions;
using SharedService.Domain.FeatureExtensions;

namespace SharedService.Infrastructure.Services.FeatureExtensions;

internal static class FeatureExtension09Mappings
{
    public static FacilityServicePriceListResponseDto ToDto(this FacilityServicePriceList e) =>
        new()
        {
            Id = e.Id,
            FacilityId = e.FacilityId,
            PriceListCode = e.PriceListCode,
            PriceListName = e.PriceListName,
            ServiceModule = e.ServiceModule,
            PartnerReferenceCode = e.PartnerReferenceCode,
            CurrencyCode = e.CurrencyCode,
            EffectiveFrom = e.EffectiveFrom,
            EffectiveTo = e.EffectiveTo,
            IsActive = e.IsActive
        };

    public static FacilityServicePriceListLineResponseDto ToDto(this FacilityServicePriceListLine e) =>
        new()
        {
            Id = e.Id,
            FacilityId = e.FacilityId,
            PriceListId = e.PriceListId,
            ServiceItemCode = e.ServiceItemCode,
            ServiceItemName = e.ServiceItemName,
            UnitPrice = e.UnitPrice,
            TaxCategoryCode = e.TaxCategoryCode,
            IsActive = e.IsActive
        };

    public static CrossFacilityReportAuditResponseDto ToDto(this CrossFacilityReportAudit e) =>
        new()
        {
            Id = e.Id,
            FacilityId = e.FacilityId,
            ReportCode = e.ReportCode,
            ReportName = e.ReportName,
            FacilityScopeJson = e.FacilityScopeJson,
            FilterJson = e.FilterJson,
            ResultRowCount = e.ResultRowCount,
            CompletedOn = e.CompletedOn,
            IsActive = e.IsActive
        };

    public static ModuleIntegrationHandoffResponseDto ToDto(this ModuleIntegrationHandoff e) =>
        new()
        {
            Id = e.Id,
            FacilityId = e.FacilityId,
            CorrelationId = e.CorrelationId,
            SourceModule = e.SourceModule,
            TargetModule = e.TargetModule,
            EntityType = e.EntityType,
            SourceEntityId = e.SourceEntityId,
            TargetEntityId = e.TargetEntityId,
            StatusCode = e.StatusCode,
            DetailJson = e.DetailJson,
            IsActive = e.IsActive
        };

    public static TenantOnboardingStageResponseDto ToDto(this TenantOnboardingStage e) =>
        new()
        {
            Id = e.Id,
            FacilityId = e.FacilityId,
            StageCode = e.StageCode,
            StageStatus = e.StageStatus,
            CompletedOn = e.CompletedOn,
            MetadataJson = e.MetadataJson,
            IsActive = e.IsActive
        };

    public static LabCriticalValueEscalationResponseDto ToDto(this LabCriticalValueEscalation e) =>
        new()
        {
            Id = e.Id,
            FacilityId = e.FacilityId,
            LabOrderId = e.LabOrderId,
            LabOrderItemId = e.LabOrderItemId,
            LabResultId = e.LabResultId,
            EscalationLevel = e.EscalationLevel,
            ChannelCode = e.ChannelCode,
            RecipientSummary = e.RecipientSummary,
            DispatchedOn = e.DispatchedOn,
            AcknowledgedOn = e.AcknowledgedOn,
            OutcomeCode = e.OutcomeCode,
            IsActive = e.IsActive
        };
}
