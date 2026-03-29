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

public interface ILmsProcessingStageService
{
    Task<BaseResponse<ProcessingStageResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<ProcessingStageResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<ProcessingStageResponseDto>> CreateAsync(CreateProcessingStageDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<ProcessingStageResponseDto>> UpdateAsync(long id, UpdateProcessingStageDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsProcessingStageService : LmsCrudServiceBase<LmsProcessingStage, CreateProcessingStageDto, UpdateProcessingStageDto, ProcessingStageResponseDto, LmsProcessingStageService>, ILmsProcessingStageService
{
    public LmsProcessingStageService(
        IRepository<LmsProcessingStage> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateProcessingStageDto>? createValidator,
        IValidator<UpdateProcessingStageDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<LmsProcessingStageService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<ProcessingStageResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
