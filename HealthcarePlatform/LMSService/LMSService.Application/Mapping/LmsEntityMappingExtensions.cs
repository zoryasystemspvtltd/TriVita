using AutoMapper;
using Healthcare.Common.Entities;

namespace LMSService.Application.Mapping;

internal static class LmsEntityMappingExtensions
{
    public static IMappingExpression<TDto, TEntity> IgnoreBaseEntityOnCreate<TDto, TEntity>(
        this IMappingExpression<TDto, TEntity> expr)
        where TEntity : BaseEntity =>
        expr
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

    public static IMappingExpression<TDto, TEntity> IgnoreBaseEntityOnUpdate<TDto, TEntity>(
        this IMappingExpression<TDto, TEntity> expr)
        where TEntity : BaseEntity => expr.IgnoreBaseEntityOnCreate();
}
