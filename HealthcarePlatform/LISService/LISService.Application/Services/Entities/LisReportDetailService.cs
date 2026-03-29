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

public interface ILisReportDetailService
{
    Task<BaseResponse<ReportDetailResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<ReportDetailResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<ReportDetailResponseDto>> CreateAsync(CreateReportDetailDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<ReportDetailResponseDto>> UpdateAsync(long id, UpdateReportDetailDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LisReportDetailService : LisCrudServiceBase<LisReportDetail, CreateReportDetailDto, UpdateReportDetailDto, ReportDetailResponseDto, LisReportDetailService>, ILisReportDetailService
{
    public LisReportDetailService(
        IRepository<LisReportDetail> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateReportDetailDto>? createValidator,
        IValidator<UpdateReportDetailDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<LisReportDetailService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<ReportDetailResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
