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

public interface ILmsQcRecordService
{
    Task<BaseResponse<QcRecordResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<QcRecordResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<QcRecordResponseDto>> CreateAsync(CreateQcRecordDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<QcRecordResponseDto>> UpdateAsync(long id, UpdateQcRecordDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsQcRecordService : LmsCrudServiceBase<LmsQcRecord, CreateQcRecordDto, UpdateQcRecordDto, QcRecordResponseDto, LmsQcRecordService>, ILmsQcRecordService
{
    public LmsQcRecordService(
        IRepository<LmsQcRecord> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateQcRecordDto>? createValidator,
        IValidator<UpdateQcRecordDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<LmsQcRecordService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<QcRecordResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
