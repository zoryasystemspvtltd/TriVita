using AutoMapper;
using LMSService.Application.DTOs.Entities;
using LMSService.Domain.Entities;

namespace LMSService.Application.Mapping;

public sealed class LmsScript10MappingProfile : Profile
{
    public LmsScript10MappingProfile()
    {
        CreateMap<LmsCollectionRequest, LmsCollectionRequestResponseDto>()
            .ForMember(d => d.FacilityId, o => o.MapFrom(s => s.FacilityId ?? 0));
        CreateMap<CreateLmsCollectionRequestDto, LmsCollectionRequest>().ApplyLmsScript10CreateIgnores()
            .ForMember(d => d.RequestNo, o => o.Ignore());
        CreateMap<UpdateLmsCollectionRequestDto, LmsCollectionRequest>().ApplyLmsScript10UpdateIgnores()
            .ForMember(d => d.RequestNo, o => o.Ignore())
            .ForMember(d => d.PatientId, o => o.Ignore());

        CreateMap<LmsRiderTracking, LmsRiderTrackingResponseDto>();
        CreateMap<CreateLmsRiderTrackingDto, LmsRiderTracking>().ApplyLmsScript10CreateIgnores();
        CreateMap<UpdateLmsRiderTrackingDto, LmsRiderTracking>().ApplyLmsScript10UpdateIgnores()
            .ForMember(d => d.CollectionRequestId, o => o.Ignore());

        CreateMap<LmsSampleTransport, LmsSampleTransportResponseDto>();
        CreateMap<CreateLmsSampleTransportDto, LmsSampleTransport>().ApplyLmsScript10CreateIgnores();
        CreateMap<UpdateLmsSampleTransportDto, LmsSampleTransport>().ApplyLmsScript10UpdateIgnores()
            .ForMember(d => d.CollectionRequestId, o => o.Ignore());

        CreateMap<LmsReportValidationStep, LmsReportValidationStepResponseDto>();
        CreateMap<CreateLmsReportValidationStepDto, LmsReportValidationStep>().ApplyLmsScript10CreateIgnores();
        CreateMap<UpdateLmsReportValidationStepDto, LmsReportValidationStep>().ApplyLmsScript10UpdateIgnores()
            .ForMember(d => d.LabOrderId, o => o.Ignore());

        CreateMap<LmsResultDeltaCheck, LmsResultDeltaCheckResponseDto>();
        CreateMap<CreateLmsResultDeltaCheckDto, LmsResultDeltaCheck>().ApplyLmsScript10CreateIgnores();
        CreateMap<UpdateLmsResultDeltaCheckDto, LmsResultDeltaCheck>().ApplyLmsScript10UpdateIgnores()
            .ForMember(d => d.CurrentLabResultId, o => o.Ignore())
            .ForMember(d => d.PriorLabResultId, o => o.Ignore());

        CreateMap<LmsReportDigitalSign, LmsReportDigitalSignResponseDto>();
        CreateMap<CreateLmsReportDigitalSignDto, LmsReportDigitalSign>().ApplyLmsScript10CreateIgnores();
        CreateMap<UpdateLmsReportDigitalSignDto, LmsReportDigitalSign>().ApplyLmsScript10UpdateIgnores()
            .ForMember(d => d.ReportHeaderId, o => o.Ignore());
    }
}

internal static class LmsScript10MappingExtensions
{
    public static IMappingExpression<TS, TD> ApplyLmsScript10CreateIgnores<TS, TD>(this IMappingExpression<TS, TD> m)
        where TD : Healthcare.Common.Entities.BaseEntity =>
        m
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore());

    public static IMappingExpression<TS, TD> ApplyLmsScript10UpdateIgnores<TS, TD>(this IMappingExpression<TS, TD> m)
        where TD : Healthcare.Common.Entities.BaseEntity =>
        m
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.TenantId, o => o.Ignore())
            .ForMember(d => d.FacilityId, o => o.Ignore())
            .ForMember(d => d.IsDeleted, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.RowVersion, o => o.Ignore());
}
