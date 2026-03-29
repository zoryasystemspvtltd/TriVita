using System.Linq.Expressions;
using AutoMapper;
using FluentValidation;
using Healthcare.Common.Entities;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using PharmacyService.Application.Services;
using PharmacyService.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace PharmacyService.Application.Services.Extended;

public abstract class PhrCrudServiceBase<TEntity, TCreate, TUpdate, TResponse, TService>
    where TEntity : BaseEntity
    where TService : class
{
    protected readonly IRepository<TEntity> Repository;
    protected readonly IMapper Mapper;
    protected readonly ITenantContext Tenant;
    protected readonly IFacilityTenantValidator FacilityValidator;
    private readonly IValidator<TCreate>? _createValidator;
    private readonly IValidator<TUpdate>? _updateValidator;
    protected readonly ILogger<TService> Logger;

    protected PhrCrudServiceBase(
        IRepository<TEntity> repository,
        IMapper mapper,
        ITenantContext tenant,
        IValidator<TCreate>? createValidator,
        IValidator<TUpdate>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<TService> logger)
    {
        Repository = repository;
        Mapper = mapper;
        Tenant = tenant;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        FacilityValidator = facilityValidator;
        Logger = logger;
    }

    protected virtual bool RequiresFacilityId => true;

    public virtual async Task<BaseResponse<TResponse>> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        Logger.LogInformation(
            "Pharmacy {Entity} GetById tenant {TenantId} id {EntityId}",
            typeof(TEntity).Name,
            Tenant.TenantId,
            id);

        var entity = await Repository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return BaseResponse<TResponse>.Fail($"{typeof(TEntity).Name} not found.");

        if (!IsEntityInFacilityScope(entity))
            return BaseResponse<TResponse>.Fail("Resource is not in the current facility scope.");

        return BaseResponse<TResponse>.Ok(Mapper.Map<TResponse>(entity));
    }

    protected async Task<BaseResponse<PagedResponse<TResponse>>> GetPagedCoreAsync(
        PagedQuery query,
        Expression<Func<TEntity, bool>>? extraFilter,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation(
            "Pharmacy {Entity} GetPaged tenant {TenantId} page {Page}",
            typeof(TEntity).Name,
            Tenant.TenantId,
            query.Page);

        Expression<Func<TEntity, bool>>? scopedFilter = extraFilter;
        if (RequiresFacilityId && Tenant.FacilityId is { } scopedF)
        {
            Expression<Func<TEntity, bool>> fac = e => e.FacilityId == scopedF;
            scopedFilter = extraFilter is null ? fac : AndAlsoFilters(extraFilter, fac);
        }

        var (items, total) = await Repository.GetPagedByFilterAsync(
            query.Page,
            query.PageSize,
            scopedFilter,
            cancellationToken);

        var dtoItems = Mapper.Map<IReadOnlyList<TResponse>>(items);
        var paged = new PagedResponse<TResponse>
        {
            Items = dtoItems,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = total
        };

        return BaseResponse<PagedResponse<TResponse>>.Ok(paged);
    }

    public virtual async Task<BaseResponse<TResponse>> CreateAsync(TCreate dto, CancellationToken cancellationToken = default)
    {
        if (RequiresFacilityId && Tenant.FacilityId is null)
            return BaseResponse<TResponse>.Fail("FacilityId is required (header X-Facility-Id or claim facility_id).");

        if (RequiresFacilityId && Tenant.FacilityId is long fid)
        {
            var ctx = await FacilityValidator.GetFacilityContextAsync(Tenant.TenantId, fid, cancellationToken);
            if (ctx is null)
            {
                Logger.LogWarning(
                    "Pharmacy {Entity} Create blocked: invalid facility TenantId={TenantId} FacilityId={FacilityId}",
                    typeof(TEntity).Name,
                    Tenant.TenantId,
                    fid);
                return BaseResponse<TResponse>.Fail(
                    "Facility is not valid for this tenant (shared enterprise hierarchy).");
            }
        }

        if (_createValidator is not null)
        {
            var validation = await _createValidator.ValidateAsync(dto, cancellationToken);
            if (!validation.IsValid)
                return BaseResponse<TResponse>.Fail(
                    "Validation failed.",
                    validation.Errors.Select(e => e.ErrorMessage));
        }

        var entity = Mapper.Map<TEntity>(dto);
        AuditHelper.ApplyCreate(entity, Tenant);
        if (RequiresFacilityId)
            entity.FacilityId = Tenant.FacilityId;

        await OnBeforeCreateAsync(entity, dto, cancellationToken);

        await Repository.AddAsync(entity, cancellationToken);
        await Repository.SaveChangesAsync(cancellationToken);

        Logger.LogInformation(
            "Pharmacy {Entity} created tenant {TenantId} id {EntityId}",
            typeof(TEntity).Name,
            Tenant.TenantId,
            entity.Id);

        return BaseResponse<TResponse>.Ok(Mapper.Map<TResponse>(entity), "Created.");
    }

    protected virtual Task OnBeforeCreateAsync(TEntity entity, TCreate dto, CancellationToken cancellationToken) =>
        Task.CompletedTask;

    public virtual async Task<BaseResponse<TResponse>> UpdateAsync(long id, TUpdate dto, CancellationToken cancellationToken = default)
    {
        if (_updateValidator is not null)
        {
            var validation = await _updateValidator.ValidateAsync(dto, cancellationToken);
            if (!validation.IsValid)
                return BaseResponse<TResponse>.Fail(
                    "Validation failed.",
                    validation.Errors.Select(e => e.ErrorMessage));
        }

        var entity = await Repository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return BaseResponse<TResponse>.Fail($"{typeof(TEntity).Name} not found.");

        if (!IsEntityInFacilityScope(entity))
            return BaseResponse<TResponse>.Fail("Resource is not in the current facility scope.");

        Mapper.Map(dto, entity);
        AuditHelper.ApplyUpdate(entity, Tenant);

        await Repository.UpdateAsync(entity, cancellationToken);
        await Repository.SaveChangesAsync(cancellationToken);

        Logger.LogInformation(
            "Pharmacy {Entity} updated tenant {TenantId} id {EntityId}",
            typeof(TEntity).Name,
            Tenant.TenantId,
            id);

        return BaseResponse<TResponse>.Ok(Mapper.Map<TResponse>(entity), "Updated.");
    }

    public virtual async Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await Repository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return BaseResponse<object?>.Fail($"{typeof(TEntity).Name} not found.");

        if (!IsEntityInFacilityScope(entity))
            return BaseResponse<object?>.Fail("Resource is not in the current facility scope.");

        entity.IsDeleted = true;
        entity.IsActive = false;
        AuditHelper.ApplyUpdate(entity, Tenant);

        await Repository.UpdateAsync(entity, cancellationToken);
        await Repository.SaveChangesAsync(cancellationToken);

        Logger.LogInformation(
            "Pharmacy {Entity} soft-deleted tenant {TenantId} id {EntityId}",
            typeof(TEntity).Name,
            Tenant.TenantId,
            id);

        return BaseResponse<object?>.Ok(null, "Deleted.");
    }

    protected bool IsEntityInFacilityScope(TEntity entity)
    {
        if (!RequiresFacilityId || Tenant.FacilityId is null)
            return true;
        return entity.FacilityId is null || entity.FacilityId == Tenant.FacilityId;
    }

    private static Expression<Func<TEntity, bool>> AndAlsoFilters(
        Expression<Func<TEntity, bool>> first,
        Expression<Func<TEntity, bool>> second)
    {
        var param = Expression.Parameter(typeof(TEntity), "e");
        var left = new PhrReplaceParameterVisitor(first.Parameters[0], param).Visit(first.Body);
        var right = new PhrReplaceParameterVisitor(second.Parameters[0], param).Visit(second.Body);
        return Expression.Lambda<Func<TEntity, bool>>(Expression.AndAlso(left!, right!), param);
    }

    private sealed class PhrReplaceParameterVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _from;
        private readonly ParameterExpression _to;

        public PhrReplaceParameterVisitor(ParameterExpression from, ParameterExpression to)
        {
            _from = from;
            _to = to;
        }

        protected override Expression VisitParameter(ParameterExpression node) =>
            node == _from ? _to : base.VisitParameter(node);
    }
}
