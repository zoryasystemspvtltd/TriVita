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

public interface ILmsEquipmentCalibrationService
{
    Task<BaseResponse<EquipmentCalibrationResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<EquipmentCalibrationResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<EquipmentCalibrationResponseDto>> CreateAsync(CreateEquipmentCalibrationDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<EquipmentCalibrationResponseDto>> UpdateAsync(long id, UpdateEquipmentCalibrationDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsEquipmentCalibrationService : LmsCrudServiceBase<LmsEquipmentCalibration, CreateEquipmentCalibrationDto, UpdateEquipmentCalibrationDto, EquipmentCalibrationResponseDto, LmsEquipmentCalibrationService>, ILmsEquipmentCalibrationService
{
    public LmsEquipmentCalibrationService(
        IRepository<LmsEquipmentCalibration> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateEquipmentCalibrationDto>? createValidator,
        IValidator<UpdateEquipmentCalibrationDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<LmsEquipmentCalibrationService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<EquipmentCalibrationResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
