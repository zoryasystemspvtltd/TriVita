using AutoMapper;
using FluentValidation;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using PharmacyService.Domain.Entities;
using PharmacyService.Application.DTOs.Entities;
using PharmacyService.Domain.Repositories;
using PharmacyService.Application.Services.Extended;
using Microsoft.Extensions.Logging;

namespace PharmacyService.Application.Services.Entities;

public interface IPhrStockAdjustmentService
{
    Task<BaseResponse<StockAdjustmentResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<StockAdjustmentResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<StockAdjustmentResponseDto>> CreateAsync(CreateStockAdjustmentDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<StockAdjustmentResponseDto>> UpdateAsync(long id, UpdateStockAdjustmentDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class PhrStockAdjustmentService : PhrCrudServiceBase<PhrStockAdjustment, CreateStockAdjustmentDto, UpdateStockAdjustmentDto, StockAdjustmentResponseDto, PhrStockAdjustmentService>, IPhrStockAdjustmentService
{
    public PhrStockAdjustmentService(
        IRepository<PhrStockAdjustment> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateStockAdjustmentDto>? createValidator,
        IValidator<UpdateStockAdjustmentDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<PhrStockAdjustmentService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<StockAdjustmentResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
