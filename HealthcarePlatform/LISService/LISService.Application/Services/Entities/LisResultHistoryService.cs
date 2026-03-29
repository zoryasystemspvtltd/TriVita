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

public interface ILisResultHistoryService
{
    Task<BaseResponse<ResultHistoryResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<ResultHistoryResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<ResultHistoryResponseDto>> CreateAsync(CreateResultHistoryDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<ResultHistoryResponseDto>> UpdateAsync(long id, UpdateResultHistoryDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LisResultHistoryService : LisCrudServiceBase<LisResultHistory, CreateResultHistoryDto, UpdateResultHistoryDto, ResultHistoryResponseDto, LisResultHistoryService>, ILisResultHistoryService
{
    public LisResultHistoryService(
        IRepository<LisResultHistory> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateResultHistoryDto>? createValidator,
        IValidator<UpdateResultHistoryDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<LisResultHistoryService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<ResultHistoryResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
