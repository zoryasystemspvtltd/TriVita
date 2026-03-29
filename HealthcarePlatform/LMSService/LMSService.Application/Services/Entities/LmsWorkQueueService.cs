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

public interface ILmsWorkQueueService
{
    Task<BaseResponse<WorkQueueResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<WorkQueueResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<WorkQueueResponseDto>> CreateAsync(CreateWorkQueueDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<WorkQueueResponseDto>> UpdateAsync(long id, UpdateWorkQueueDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsWorkQueueService : LmsCrudServiceBase<LmsWorkQueue, CreateWorkQueueDto, UpdateWorkQueueDto, WorkQueueResponseDto, LmsWorkQueueService>, ILmsWorkQueueService
{
    public LmsWorkQueueService(
        IRepository<LmsWorkQueue> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateWorkQueueDto>? createValidator,
        IValidator<UpdateWorkQueueDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<LmsWorkQueueService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<WorkQueueResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
