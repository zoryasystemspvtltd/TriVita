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

public interface IPhrStockTransferService
{
    Task<BaseResponse<StockTransferResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<StockTransferResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<StockTransferResponseDto>> CreateAsync(CreateStockTransferDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<StockTransferResponseDto>> UpdateAsync(long id, UpdateStockTransferDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class PhrStockTransferService : PhrCrudServiceBase<PhrStockTransfer, CreateStockTransferDto, UpdateStockTransferDto, StockTransferResponseDto, PhrStockTransferService>, IPhrStockTransferService
{
    public PhrStockTransferService(
        IRepository<PhrStockTransfer> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateStockTransferDto>? createValidator,
        IValidator<UpdateStockTransferDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<PhrStockTransferService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<StockTransferResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
