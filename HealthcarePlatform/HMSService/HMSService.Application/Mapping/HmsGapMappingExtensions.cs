using AutoMapper;
using Healthcare.Common.Entities;

namespace HMSService.Application.Mapping;

internal static class HmsGapMappingExtensions
{
    public static IMappingExpression<TSource, TDest> ApplyStandardHmsGapIgnores<TSource, TDest>(
        this IMappingExpression<TSource, TDest> map)
        where TDest : BaseEntity =>
        map
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

    public static IMappingExpression<TSource, TDest> ApplyStandardHmsGapUpdateIgnores<TSource, TDest>(
        this IMappingExpression<TSource, TDest> map)
        where TDest : BaseEntity =>
        map
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
