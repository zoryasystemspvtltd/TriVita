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

public interface ILisLabOrderItemService
{
    Task<BaseResponse<LabOrderItemResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<LabOrderItemResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<LabOrderItemResponseDto>> CreateAsync(CreateLabOrderItemDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<LabOrderItemResponseDto>> UpdateAsync(long id, UpdateLabOrderItemDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LisLabOrderItemService : LisCrudServiceBase<LisLabOrderItem, CreateLabOrderItemDto, UpdateLabOrderItemDto, LabOrderItemResponseDto, LisLabOrderItemService>, ILisLabOrderItemService
{
    public LisLabOrderItemService(
        IRepository<LisLabOrderItem> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateLabOrderItemDto>? createValidator,
        IValidator<UpdateLabOrderItemDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<LisLabOrderItemService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<LabOrderItemResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
