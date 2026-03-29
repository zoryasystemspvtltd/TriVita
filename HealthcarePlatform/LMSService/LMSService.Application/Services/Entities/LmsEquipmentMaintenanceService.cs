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

public interface ILmsEquipmentMaintenanceService
{
    Task<BaseResponse<EquipmentMaintenanceResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<EquipmentMaintenanceResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<EquipmentMaintenanceResponseDto>> CreateAsync(CreateEquipmentMaintenanceDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<EquipmentMaintenanceResponseDto>> UpdateAsync(long id, UpdateEquipmentMaintenanceDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsEquipmentMaintenanceService : LmsCrudServiceBase<LmsEquipmentMaintenance, CreateEquipmentMaintenanceDto, UpdateEquipmentMaintenanceDto, EquipmentMaintenanceResponseDto, LmsEquipmentMaintenanceService>, ILmsEquipmentMaintenanceService
{
    public LmsEquipmentMaintenanceService(
        IRepository<LmsEquipmentMaintenance> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateEquipmentMaintenanceDto>? createValidator,
        IValidator<UpdateEquipmentMaintenanceDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<LmsEquipmentMaintenanceService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<EquipmentMaintenanceResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
