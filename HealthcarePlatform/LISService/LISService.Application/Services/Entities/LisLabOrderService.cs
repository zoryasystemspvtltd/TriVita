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

public interface ILisLabOrderService
{
    Task<BaseResponse<LabOrderResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<LabOrderResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<LabOrderResponseDto>> CreateAsync(CreateLabOrderDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<LabOrderResponseDto>> UpdateAsync(long id, UpdateLabOrderDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LisLabOrderService : LisCrudServiceBase<LisLabOrder, CreateLabOrderDto, UpdateLabOrderDto, LabOrderResponseDto, LisLabOrderService>, ILisLabOrderService
{
    public LisLabOrderService(
        IRepository<LisLabOrder> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateLabOrderDto>? createValidator,
        IValidator<UpdateLabOrderDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<LisLabOrderService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<LabOrderResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
