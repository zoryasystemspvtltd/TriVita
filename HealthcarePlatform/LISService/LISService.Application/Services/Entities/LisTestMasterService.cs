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

public interface ILisTestMasterService
{
    Task<BaseResponse<TestMasterResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<TestMasterResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<TestMasterResponseDto>> CreateAsync(CreateTestMasterDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<TestMasterResponseDto>> UpdateAsync(long id, UpdateTestMasterDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LisTestMasterService : LisCrudServiceBase<LisTestMaster, CreateTestMasterDto, UpdateTestMasterDto, TestMasterResponseDto, LisTestMasterService>, ILisTestMasterService
{
    public LisTestMasterService(
        IRepository<LisTestMaster> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateTestMasterDto>? createValidator,
        IValidator<UpdateTestMasterDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<LisTestMasterService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => false;

    public Task<BaseResponse<PagedResponse<TestMasterResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
