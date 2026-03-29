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

public interface ILmsTechnicianAssignmentService
{
    Task<BaseResponse<TechnicianAssignmentResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<TechnicianAssignmentResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<TechnicianAssignmentResponseDto>> CreateAsync(CreateTechnicianAssignmentDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<TechnicianAssignmentResponseDto>> UpdateAsync(long id, UpdateTechnicianAssignmentDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsTechnicianAssignmentService : LmsCrudServiceBase<LmsTechnicianAssignment, CreateTechnicianAssignmentDto, UpdateTechnicianAssignmentDto, TechnicianAssignmentResponseDto, LmsTechnicianAssignmentService>, ILmsTechnicianAssignmentService
{
    public LmsTechnicianAssignmentService(
        IRepository<LmsTechnicianAssignment> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateTechnicianAssignmentDto>? createValidator,
        IValidator<UpdateTechnicianAssignmentDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<LmsTechnicianAssignmentService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<TechnicianAssignmentResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
