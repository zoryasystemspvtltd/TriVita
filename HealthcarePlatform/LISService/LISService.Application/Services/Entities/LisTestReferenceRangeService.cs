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

public interface ILisTestReferenceRangeService
{
    Task<BaseResponse<TestReferenceRangeResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<TestReferenceRangeResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<TestReferenceRangeResponseDto>> CreateAsync(CreateTestReferenceRangeDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<TestReferenceRangeResponseDto>> UpdateAsync(long id, UpdateTestReferenceRangeDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LisTestReferenceRangeService : LisCrudServiceBase<LisTestReferenceRange, CreateTestReferenceRangeDto, UpdateTestReferenceRangeDto, TestReferenceRangeResponseDto, LisTestReferenceRangeService>, ILisTestReferenceRangeService
{
    public LisTestReferenceRangeService(
        IRepository<LisTestReferenceRange> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateTestReferenceRangeDto>? createValidator,
        IValidator<UpdateTestReferenceRangeDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<LisTestReferenceRangeService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => false;

    public Task<BaseResponse<PagedResponse<TestReferenceRangeResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
