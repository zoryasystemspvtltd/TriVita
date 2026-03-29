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

public interface IPhrBatchStockService
{
    Task<BaseResponse<BatchStockResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<BatchStockResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<BatchStockResponseDto>> CreateAsync(CreateBatchStockDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<BatchStockResponseDto>> UpdateAsync(long id, UpdateBatchStockDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class PhrBatchStockService : PhrCrudServiceBase<PhrBatchStock, CreateBatchStockDto, UpdateBatchStockDto, BatchStockResponseDto, PhrBatchStockService>, IPhrBatchStockService
{
    public PhrBatchStockService(
        IRepository<PhrBatchStock> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateBatchStockDto>? createValidator,
        IValidator<UpdateBatchStockDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<PhrBatchStockService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<BatchStockResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
