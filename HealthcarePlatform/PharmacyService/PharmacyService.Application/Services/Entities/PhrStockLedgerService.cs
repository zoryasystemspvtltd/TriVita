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

public interface IPhrStockLedgerService
{
    Task<BaseResponse<StockLedgerResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<StockLedgerResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<StockLedgerResponseDto>> CreateAsync(CreateStockLedgerDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<StockLedgerResponseDto>> UpdateAsync(long id, UpdateStockLedgerDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class PhrStockLedgerService : PhrCrudServiceBase<PhrStockLedger, CreateStockLedgerDto, UpdateStockLedgerDto, StockLedgerResponseDto, PhrStockLedgerService>, IPhrStockLedgerService
{
    public PhrStockLedgerService(
        IRepository<PhrStockLedger> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateStockLedgerDto>? createValidator,
        IValidator<UpdateStockLedgerDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<PhrStockLedgerService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<StockLedgerResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
