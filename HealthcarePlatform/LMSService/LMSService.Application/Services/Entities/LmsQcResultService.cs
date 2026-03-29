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

public interface ILmsQcResultService
{
    Task<BaseResponse<QcResultResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<QcResultResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<QcResultResponseDto>> CreateAsync(CreateQcResultDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<QcResultResponseDto>> UpdateAsync(long id, UpdateQcResultDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsQcResultService : LmsCrudServiceBase<LmsQcResult, CreateQcResultDto, UpdateQcResultDto, QcResultResponseDto, LmsQcResultService>, ILmsQcResultService
{
    public LmsQcResultService(
        IRepository<LmsQcResult> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateQcResultDto>? createValidator,
        IValidator<UpdateQcResultDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<LmsQcResultService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<QcResultResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
