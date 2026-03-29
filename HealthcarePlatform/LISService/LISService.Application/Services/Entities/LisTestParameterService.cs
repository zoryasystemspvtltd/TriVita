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

public interface ILisTestParameterService
{
    Task<BaseResponse<TestParameterResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<TestParameterResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<TestParameterResponseDto>> CreateAsync(CreateTestParameterDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<TestParameterResponseDto>> UpdateAsync(long id, UpdateTestParameterDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LisTestParameterService : LisCrudServiceBase<LisTestParameter, CreateTestParameterDto, UpdateTestParameterDto, TestParameterResponseDto, LisTestParameterService>, ILisTestParameterService
{
    public LisTestParameterService(
        IRepository<LisTestParameter> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateTestParameterDto>? createValidator,
        IValidator<UpdateTestParameterDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<LisTestParameterService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => false;

    public Task<BaseResponse<PagedResponse<TestParameterResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
