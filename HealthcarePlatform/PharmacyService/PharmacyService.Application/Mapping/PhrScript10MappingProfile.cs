using AutoMapper;
using PharmacyService.Application.DTOs.Entities;
using PharmacyService.Domain.Entities;

namespace PharmacyService.Application.Mapping;

public sealed class PhrScript10MappingProfile : Profile
{
    public PhrScript10MappingProfile()
    {
        CreateMap<PhrInventoryLocation, PhrInventoryLocationResponseDto>()
            .ForMember(d => d.FacilityId, o => o.MapFrom(s => s.FacilityId ?? 0));
        CreateMap<CreatePhrInventoryLocationDto, PhrInventoryLocation>().ApplyPhrScript10CreateIgnores();
        CreateMap<UpdatePhrInventoryLocationDto, PhrInventoryLocation>().ApplyPhrScript10UpdateIgnores()
            .ForMember(d => d.LocationCode, o => o.Ignore());

        CreateMap<PhrSalesReturn, PhrSalesReturnResponseDto>()
            .ForMember(d => d.FacilityId, o => o.MapFrom(s => s.FacilityId ?? 0));
        CreateMap<CreatePhrSalesReturnDto, PhrSalesReturn>().ApplyPhrScript10CreateIgnores()
            .ForMember(d => d.ReturnNo, o => o.Ignore());
        CreateMap<UpdatePhrSalesReturnDto, PhrSalesReturn>().ApplyPhrScript10UpdateIgnores()
            .ForMember(d => d.ReturnNo, o => o.Ignore())
            .ForMember(d => d.OriginalSalesId, o => o.Ignore());

        CreateMap<PhrSalesReturnItem, PhrSalesReturnItemResponseDto>();
        CreateMap<CreatePhrSalesReturnItemDto, PhrSalesReturnItem>().ApplyPhrScript10CreateIgnores();
        CreateMap<UpdatePhrSalesReturnItemDto, PhrSalesReturnItem>().ApplyPhrScript10UpdateIgnores()
            .ForMember(d => d.SalesReturnId, o => o.Ignore())
            .ForMember(d => d.OriginalSalesItemId, o => o.Ignore());

        CreateMap<PhrControlledDrugRegister, PhrControlledDrugRegisterResponseDto>();
        CreateMap<CreatePhrControlledDrugRegisterDto, PhrControlledDrugRegister>().ApplyPhrScript10CreateIgnores()
            .ForMember(d => d.PatientAcknowledged, o => o.Ignore())
            .ForMember(d => d.PatientAcknowledgedOn, o => o.Ignore());
        CreateMap<UpdatePhrControlledDrugRegisterDto, PhrControlledDrugRegister>().ApplyPhrScript10UpdateIgnores()
            .ForMember(d => d.PharmacySalesItemId, o => o.Ignore())
            .ForMember(d => d.PrescribingDoctorId, o => o.Ignore())
            .ForMember(d => d.PatientId, o => o.Ignore())
            .ForMember(d => d.RegisterEntryOn, o => o.Ignore());

        CreateMap<PhrBatchStockLocation, PhrBatchStockLocationResponseDto>();
        CreateMap<CreatePhrBatchStockLocationDto, PhrBatchStockLocation>().ApplyPhrScript10CreateIgnores();
        CreateMap<UpdatePhrBatchStockLocationDto, PhrBatchStockLocation>().ApplyPhrScript10UpdateIgnores()
            .ForMember(d => d.BatchStockId, o => o.Ignore())
            .ForMember(d => d.InventoryLocationId, o => o.Ignore());

        CreateMap<PhrReorderPolicy, PhrReorderPolicyResponseDto>();
        CreateMap<CreatePhrReorderPolicyDto, PhrReorderPolicy>().ApplyPhrScript10CreateIgnores();
        CreateMap<UpdatePhrReorderPolicyDto, PhrReorderPolicy>().ApplyPhrScript10UpdateIgnores()
            .ForMember(d => d.BatchStockId, o => o.Ignore());
    }
}

internal static class PhrScript10MappingExtensions
{
    public static IMappingExpression<TS, TD> ApplyPhrScript10CreateIgnores<TS, TD>(this IMappingExpression<TS, TD> m)
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

    public static IMappingExpression<TS, TD> ApplyPhrScript10UpdateIgnores<TS, TD>(this IMappingExpression<TS, TD> m)
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
