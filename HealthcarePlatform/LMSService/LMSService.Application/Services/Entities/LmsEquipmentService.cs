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

public interface ILmsEquipmentService
{
    Task<BaseResponse<EquipmentResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<EquipmentResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<EquipmentResponseDto>> CreateAsync(CreateEquipmentDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<EquipmentResponseDto>> UpdateAsync(long id, UpdateEquipmentDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsEquipmentService : LmsCrudServiceBase<LmsEquipment, CreateEquipmentDto, UpdateEquipmentDto, EquipmentResponseDto, LmsEquipmentService>, ILmsEquipmentService
{
    public LmsEquipmentService(
        IRepository<LmsEquipment> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateEquipmentDto>? createValidator,
        IValidator<UpdateEquipmentDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<LmsEquipmentService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<EquipmentResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
