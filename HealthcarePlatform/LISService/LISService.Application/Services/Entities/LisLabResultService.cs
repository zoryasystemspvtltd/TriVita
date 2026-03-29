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

public interface ILisLabResultService
{
    Task<BaseResponse<LabResultResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<LabResultResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<LabResultResponseDto>> CreateAsync(CreateLabResultDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<LabResultResponseDto>> UpdateAsync(long id, UpdateLabResultDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LisLabResultService : LisCrudServiceBase<LisLabResult, CreateLabResultDto, UpdateLabResultDto, LabResultResponseDto, LisLabResultService>, ILisLabResultService
{
    public LisLabResultService(
        IRepository<LisLabResult> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateLabResultDto>? createValidator,
        IValidator<UpdateLabResultDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<LisLabResultService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<LabResultResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
