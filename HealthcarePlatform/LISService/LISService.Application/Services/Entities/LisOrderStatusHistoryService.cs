using AutoMapper;
using FluentValidation;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using LISService.Domain.Entities;
using LISService.Application.DTOs.Entities;
using LISService.Domain.Repositories;
using LISService.Application.Services.Extended;
using Microsoft.Extensions.Logging;

namespace LISService.Application.Services.Entities;

public interface ILisOrderStatusHistoryService
{
    Task<BaseResponse<OrderStatusHistoryResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<OrderStatusHistoryResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<OrderStatusHistoryResponseDto>> CreateAsync(CreateOrderStatusHistoryDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<OrderStatusHistoryResponseDto>> UpdateAsync(long id, UpdateOrderStatusHistoryDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LisOrderStatusHistoryService : LisCrudServiceBase<LisOrderStatusHistory, CreateOrderStatusHistoryDto, UpdateOrderStatusHistoryDto, OrderStatusHistoryResponseDto, LisOrderStatusHistoryService>, ILisOrderStatusHistoryService
{
    public LisOrderStatusHistoryService(
        IRepository<LisOrderStatusHistory> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateOrderStatusHistoryDto>? createValidator,
        IValidator<UpdateOrderStatusHistoryDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<LisOrderStatusHistoryService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<OrderStatusHistoryResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
