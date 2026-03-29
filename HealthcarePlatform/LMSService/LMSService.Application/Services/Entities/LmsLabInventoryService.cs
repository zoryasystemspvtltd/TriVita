using AutoMapper;
using FluentValidation;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using LMSService.Domain.Entities;
using LMSService.Application.DTOs.Entities;
using LMSService.Domain.Repositories;
using LMSService.Application.Services.Extended;
using Microsoft.Extensions.Logging;

namespace LMSService.Application.Services.Entities;

public interface ILmsLabInventoryService
{
    Task<BaseResponse<LabInventoryResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<LabInventoryResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<LabInventoryResponseDto>> CreateAsync(CreateLabInventoryDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<LabInventoryResponseDto>> UpdateAsync(long id, UpdateLabInventoryDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsLabInventoryService : LmsCrudServiceBase<LmsLabInventory, CreateLabInventoryDto, UpdateLabInventoryDto, LabInventoryResponseDto, LmsLabInventoryService>, ILmsLabInventoryService
{
    public LmsLabInventoryService(
        IRepository<LmsLabInventory> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateLabInventoryDto>? createValidator,
        IValidator<UpdateLabInventoryDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<LmsLabInventoryService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<LabInventoryResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
