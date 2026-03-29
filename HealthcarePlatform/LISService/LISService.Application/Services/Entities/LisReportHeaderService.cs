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

public interface ILisReportHeaderService
{
    Task<BaseResponse<ReportHeaderResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<ReportHeaderResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<ReportHeaderResponseDto>> CreateAsync(CreateReportHeaderDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<ReportHeaderResponseDto>> UpdateAsync(long id, UpdateReportHeaderDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LisReportHeaderService : LisCrudServiceBase<LisReportHeader, CreateReportHeaderDto, UpdateReportHeaderDto, ReportHeaderResponseDto, LisReportHeaderService>, ILisReportHeaderService
{
    public LisReportHeaderService(
        IRepository<LisReportHeader> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateReportHeaderDto>? createValidator,
        IValidator<UpdateReportHeaderDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<LisReportHeaderService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<ReportHeaderResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
