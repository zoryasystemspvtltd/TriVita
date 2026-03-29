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

public interface ILisTestCategoryService
{
    Task<BaseResponse<TestCategoryResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<TestCategoryResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<TestCategoryResponseDto>> CreateAsync(CreateTestCategoryDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<TestCategoryResponseDto>> UpdateAsync(long id, UpdateTestCategoryDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LisTestCategoryService : LisCrudServiceBase<LisTestCategory, CreateTestCategoryDto, UpdateTestCategoryDto, TestCategoryResponseDto, LisTestCategoryService>, ILisTestCategoryService
{
    public LisTestCategoryService(
        IRepository<LisTestCategory> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateTestCategoryDto>? createValidator,
        IValidator<UpdateTestCategoryDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<LisTestCategoryService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => false;

    public Task<BaseResponse<PagedResponse<TestCategoryResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
