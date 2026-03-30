using AutoMapper;
using FluentValidation;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using PharmacyService.Application.DTOs.Entities;
using PharmacyService.Application.Services.Extended;
using PharmacyService.Domain.Entities;
using PharmacyService.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace PharmacyService.Application.Services.Entities;

public interface IPhrInventoryLocationService
{
    Task<BaseResponse<PhrInventoryLocationResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<PhrInventoryLocationResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<PhrInventoryLocationResponseDto>> CreateAsync(CreatePhrInventoryLocationDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<PhrInventoryLocationResponseDto>> UpdateAsync(long id, UpdatePhrInventoryLocationDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class PhrInventoryLocationService
    : PhrCrudServiceBase<PhrInventoryLocation, CreatePhrInventoryLocationDto, UpdatePhrInventoryLocationDto, PhrInventoryLocationResponseDto, PhrInventoryLocationService>,
        IPhrInventoryLocationService
{
    public PhrInventoryLocationService(
        IRepository<PhrInventoryLocation> repository,
        IMapper mapper,
        ITenantContext tenant,
        IValidator<CreatePhrInventoryLocationDto>? cv,
        IValidator<UpdatePhrInventoryLocationDto>? uv,
        IFacilityTenantValidator fv,
        ILogger<PhrInventoryLocationService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger)
    {
    }

    public Task<BaseResponse<PagedResponse<PhrInventoryLocationResponseDto>>> GetPagedAsync(
        PagedQuery query,
        CancellationToken ct = default) =>
        GetPagedCoreAsync(query, null, ct);
}

public interface IPhrSalesReturnService
{
    Task<BaseResponse<PhrSalesReturnResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<PhrSalesReturnResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<PhrSalesReturnResponseDto>> CreateAsync(CreatePhrSalesReturnDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<PhrSalesReturnResponseDto>> UpdateAsync(long id, UpdatePhrSalesReturnDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class PhrSalesReturnService
    : PhrCrudServiceBase<PhrSalesReturn, CreatePhrSalesReturnDto, UpdatePhrSalesReturnDto, PhrSalesReturnResponseDto, PhrSalesReturnService>,
        IPhrSalesReturnService
{
    public PhrSalesReturnService(
        IRepository<PhrSalesReturn> repository,
        IMapper mapper,
        ITenantContext tenant,
        IValidator<CreatePhrSalesReturnDto>? cv,
        IValidator<UpdatePhrSalesReturnDto>? uv,
        IFacilityTenantValidator fv,
        ILogger<PhrSalesReturnService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger)
    {
    }

    protected override Task OnBeforeCreateAsync(
        PhrSalesReturn entity,
        CreatePhrSalesReturnDto dto,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(entity.ReturnNo))
        {
            var f = Tenant.FacilityId ?? 0;
            entity.ReturnNo = $"RET-{f}-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}";
        }

        return Task.CompletedTask;
    }

    public Task<BaseResponse<PagedResponse<PhrSalesReturnResponseDto>>> GetPagedAsync(
        PagedQuery query,
        CancellationToken ct = default) =>
        GetPagedCoreAsync(query, null, ct);
}

public interface IPhrSalesReturnItemService
{
    Task<BaseResponse<PhrSalesReturnItemResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<PhrSalesReturnItemResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? salesReturnId,
        CancellationToken cancellationToken = default);
    Task<BaseResponse<PhrSalesReturnItemResponseDto>> CreateAsync(CreatePhrSalesReturnItemDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<PhrSalesReturnItemResponseDto>> UpdateAsync(long id, UpdatePhrSalesReturnItemDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class PhrSalesReturnItemService
    : PhrCrudServiceBase<PhrSalesReturnItem, CreatePhrSalesReturnItemDto, UpdatePhrSalesReturnItemDto, PhrSalesReturnItemResponseDto, PhrSalesReturnItemService>,
        IPhrSalesReturnItemService
{
    public PhrSalesReturnItemService(
        IRepository<PhrSalesReturnItem> repository,
        IMapper mapper,
        ITenantContext tenant,
        IValidator<CreatePhrSalesReturnItemDto>? cv,
        IValidator<UpdatePhrSalesReturnItemDto>? uv,
        IFacilityTenantValidator fv,
        ILogger<PhrSalesReturnItemService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger)
    {
    }

    public Task<BaseResponse<PagedResponse<PhrSalesReturnItemResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? salesReturnId,
        CancellationToken ct = default) =>
        GetPagedCoreAsync(
            query,
            salesReturnId is null ? null : e => e.SalesReturnId == salesReturnId.Value,
            ct);
}

public interface IPhrControlledDrugRegisterService
{
    Task<BaseResponse<PhrControlledDrugRegisterResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<PhrControlledDrugRegisterResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<PhrControlledDrugRegisterResponseDto>> CreateAsync(CreatePhrControlledDrugRegisterDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<PhrControlledDrugRegisterResponseDto>> UpdateAsync(long id, UpdatePhrControlledDrugRegisterDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class PhrControlledDrugRegisterService
    : PhrCrudServiceBase<PhrControlledDrugRegister, CreatePhrControlledDrugRegisterDto, UpdatePhrControlledDrugRegisterDto, PhrControlledDrugRegisterResponseDto, PhrControlledDrugRegisterService>,
        IPhrControlledDrugRegisterService
{
    public PhrControlledDrugRegisterService(
        IRepository<PhrControlledDrugRegister> repository,
        IMapper mapper,
        ITenantContext tenant,
        IValidator<CreatePhrControlledDrugRegisterDto>? cv,
        IValidator<UpdatePhrControlledDrugRegisterDto>? uv,
        IFacilityTenantValidator fv,
        ILogger<PhrControlledDrugRegisterService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger)
    {
    }

    public Task<BaseResponse<PagedResponse<PhrControlledDrugRegisterResponseDto>>> GetPagedAsync(
        PagedQuery query,
        CancellationToken ct = default) =>
        GetPagedCoreAsync(query, null, ct);
}

public interface IPhrBatchStockLocationService
{
    Task<BaseResponse<PhrBatchStockLocationResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<PhrBatchStockLocationResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? batchStockId,
        long? inventoryLocationId,
        CancellationToken cancellationToken = default);
    Task<BaseResponse<PhrBatchStockLocationResponseDto>> CreateAsync(CreatePhrBatchStockLocationDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<PhrBatchStockLocationResponseDto>> UpdateAsync(long id, UpdatePhrBatchStockLocationDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class PhrBatchStockLocationService
    : PhrCrudServiceBase<PhrBatchStockLocation, CreatePhrBatchStockLocationDto, UpdatePhrBatchStockLocationDto, PhrBatchStockLocationResponseDto, PhrBatchStockLocationService>,
        IPhrBatchStockLocationService
{
    public PhrBatchStockLocationService(
        IRepository<PhrBatchStockLocation> repository,
        IMapper mapper,
        ITenantContext tenant,
        IValidator<CreatePhrBatchStockLocationDto>? cv,
        IValidator<UpdatePhrBatchStockLocationDto>? uv,
        IFacilityTenantValidator fv,
        ILogger<PhrBatchStockLocationService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger)
    {
    }

    public Task<BaseResponse<PagedResponse<PhrBatchStockLocationResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? batchStockId,
        long? inventoryLocationId,
        CancellationToken ct = default)
    {
        System.Linq.Expressions.Expression<Func<PhrBatchStockLocation, bool>>? pred = null;
        if (batchStockId is long b)
            pred = e => e.BatchStockId == b;
        if (inventoryLocationId is long loc)
        {
            System.Linq.Expressions.Expression<Func<PhrBatchStockLocation, bool>> p = e => e.InventoryLocationId == loc;
            pred = pred is null ? p : Combine(pred, p);
        }

        return GetPagedCoreAsync(query, pred, ct);
    }

    private static System.Linq.Expressions.Expression<Func<PhrBatchStockLocation, bool>> Combine(
        System.Linq.Expressions.Expression<Func<PhrBatchStockLocation, bool>> first,
        System.Linq.Expressions.Expression<Func<PhrBatchStockLocation, bool>> second)
    {
        var param = System.Linq.Expressions.Expression.Parameter(typeof(PhrBatchStockLocation), "e");
        var left = new ParamRepl(first.Parameters[0], param).Visit(first.Body);
        var right = new ParamRepl(second.Parameters[0], param).Visit(second.Body);
        return System.Linq.Expressions.Expression.Lambda<Func<PhrBatchStockLocation, bool>>(
            System.Linq.Expressions.Expression.AndAlso(left!, right!), param);
    }

    private sealed class ParamRepl : System.Linq.Expressions.ExpressionVisitor
    {
        private readonly System.Linq.Expressions.ParameterExpression _from;
        private readonly System.Linq.Expressions.ParameterExpression _to;

        public ParamRepl(System.Linq.Expressions.ParameterExpression from, System.Linq.Expressions.ParameterExpression to)
        {
            _from = from;
            _to = to;
        }

        protected override System.Linq.Expressions.Expression VisitParameter(
            System.Linq.Expressions.ParameterExpression node) =>
            node == _from ? _to : base.VisitParameter(node);
    }
}

public interface IPhrReorderPolicyService
{
    Task<BaseResponse<PhrReorderPolicyResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<PhrReorderPolicyResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? batchStockId,
        CancellationToken cancellationToken = default);
    Task<BaseResponse<PhrReorderPolicyResponseDto>> CreateAsync(CreatePhrReorderPolicyDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<PhrReorderPolicyResponseDto>> UpdateAsync(long id, UpdatePhrReorderPolicyDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class PhrReorderPolicyService
    : PhrCrudServiceBase<PhrReorderPolicy, CreatePhrReorderPolicyDto, UpdatePhrReorderPolicyDto, PhrReorderPolicyResponseDto, PhrReorderPolicyService>,
        IPhrReorderPolicyService
{
    public PhrReorderPolicyService(
        IRepository<PhrReorderPolicy> repository,
        IMapper mapper,
        ITenantContext tenant,
        IValidator<CreatePhrReorderPolicyDto>? cv,
        IValidator<UpdatePhrReorderPolicyDto>? uv,
        IFacilityTenantValidator fv,
        ILogger<PhrReorderPolicyService> logger)
        : base(repository, mapper, tenant, cv, uv, fv, logger)
    {
    }

    public Task<BaseResponse<PagedResponse<PhrReorderPolicyResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? batchStockId,
        CancellationToken ct = default) =>
        GetPagedCoreAsync(
            query,
            batchStockId is null ? null : e => e.BatchStockId == batchStockId.Value,
            ct);
}
