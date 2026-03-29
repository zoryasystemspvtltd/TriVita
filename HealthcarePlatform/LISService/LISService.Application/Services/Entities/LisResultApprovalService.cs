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

public interface ILisResultApprovalService
{
    Task<BaseResponse<ResultApprovalResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<ResultApprovalResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<ResultApprovalResponseDto>> CreateAsync(CreateResultApprovalDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<ResultApprovalResponseDto>> UpdateAsync(long id, UpdateResultApprovalDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LisResultApprovalService : LisCrudServiceBase<LisResultApproval, CreateResultApprovalDto, UpdateResultApprovalDto, ResultApprovalResponseDto, LisResultApprovalService>, ILisResultApprovalService
{
    public LisResultApprovalService(
        IRepository<LisResultApproval> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateResultApprovalDto>? createValidator,
        IValidator<UpdateResultApprovalDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<LisResultApprovalService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<ResultApprovalResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
