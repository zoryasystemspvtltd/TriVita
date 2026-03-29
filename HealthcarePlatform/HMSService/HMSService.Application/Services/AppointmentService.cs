using AutoMapper;
using FluentValidation;
using Healthcare.Common.Integration.SharedService;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using HMSService.Application.Abstractions;
using HMSService.Application.DTOs.Appointments;
using HMSService.Domain.Entities;
using HMSService.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace HMSService.Application.Services;

public sealed class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _repository;
    private readonly IMapper _mapper;
    private readonly ITenantContext _tenant;
    private readonly IValidator<CreateAppointmentDto> _createValidator;
    private readonly IValidator<UpdateAppointmentDto> _updateValidator;
    private readonly INotificationHelper _notificationHelper;
    private readonly IFacilityTenantValidator _facilityValidator;
    private readonly ILogger<AppointmentService> _logger;

    public AppointmentService(
        IAppointmentRepository repository,
        IMapper mapper,
        ITenantContext tenant,
        IValidator<CreateAppointmentDto> createValidator,
        IValidator<UpdateAppointmentDto> updateValidator,
        INotificationHelper notificationHelper,
        IFacilityTenantValidator facilityValidator,
        ILogger<AppointmentService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _tenant = tenant;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _notificationHelper = notificationHelper;
        _facilityValidator = facilityValidator;
        _logger = logger;
    }

    public async Task<BaseResponse<AppointmentResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return BaseResponse<AppointmentResponseDto>.Fail("Appointment not found.");

        if (!IsAppointmentInFacilityScope(entity))
            return BaseResponse<AppointmentResponseDto>.Fail("Resource is not in the current facility scope.");

        return BaseResponse<AppointmentResponseDto>.Ok(_mapper.Map<AppointmentResponseDto>(entity));
    }

    public async Task<BaseResponse<PagedResponse<AppointmentResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? patientId,
        long? doctorId,
        DateTime? scheduledFrom,
        DateTime? scheduledTo,
        CancellationToken cancellationToken = default)
    {
        var (items, total) = await _repository.GetPagedAsync(
            query.Page,
            query.PageSize,
            query.SortBy,
            query.SortDescending,
            patientId,
            doctorId,
            scheduledFrom,
            scheduledTo,
            _tenant.FacilityId,
            cancellationToken);

        var dtoItems = _mapper.Map<IReadOnlyList<AppointmentResponseDto>>(items);

        var paged = new PagedResponse<AppointmentResponseDto>
        {
            Items = dtoItems,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = total
        };

        return BaseResponse<PagedResponse<AppointmentResponseDto>>.Ok(paged);
    }

    public async Task<BaseResponse<AppointmentResponseDto>> CreateAsync(
        CreateAppointmentDto dto,
        CancellationToken cancellationToken = default)
    {
        if (_tenant.FacilityId is null)
            return BaseResponse<AppointmentResponseDto>.Fail("FacilityId is required (header X-Facility-Id or claim facility_id).");

        if (await _facilityValidator.GetFacilityContextAsync(_tenant.TenantId, _tenant.FacilityId.Value, cancellationToken) is null)
        {
            _logger.LogWarning(
                "Appointment create blocked: invalid facility TenantId={TenantId} FacilityId={FacilityId}",
                _tenant.TenantId,
                _tenant.FacilityId);
            return BaseResponse<AppointmentResponseDto>.Fail(
                "Facility is not valid for this tenant (shared enterprise hierarchy).");
        }

        var validation = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return BaseResponse<AppointmentResponseDto>.Fail(
                "Validation failed.",
                validation.Errors.Select(e => e.ErrorMessage));

        var entity = _mapper.Map<HmsAppointment>(dto);
        AuditHelper.ApplyCreate(entity, _tenant);
        entity.FacilityId = _tenant.FacilityId;
        entity.AppointmentNo = GenerateNumber("APT");

        await _repository.AddAsync(entity, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        await _notificationHelper.NotifyAppointmentCreatedAsync(entity.Id, entity.PatientId, null, cancellationToken);

        return BaseResponse<AppointmentResponseDto>.Ok(_mapper.Map<AppointmentResponseDto>(entity), "Created.");
    }

    public async Task<BaseResponse<AppointmentResponseDto>> UpdateAsync(
        long id,
        UpdateAppointmentDto dto,
        CancellationToken cancellationToken = default)
    {
        var validation = await _updateValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return BaseResponse<AppointmentResponseDto>.Fail(
                "Validation failed.",
                validation.Errors.Select(e => e.ErrorMessage));

        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return BaseResponse<AppointmentResponseDto>.Fail("Appointment not found.");

        if (!IsAppointmentInFacilityScope(entity))
            return BaseResponse<AppointmentResponseDto>.Fail("Resource is not in the current facility scope.");

        entity.DoctorId = dto.DoctorId;
        entity.DepartmentId = dto.DepartmentId;
        entity.VisitTypeId = dto.VisitTypeId;
        entity.AppointmentStatusValueId = dto.AppointmentStatusValueId;
        entity.ScheduledStartOn = dto.ScheduledStartOn;
        entity.ScheduledEndOn = dto.ScheduledEndOn;
        entity.PriorityReferenceValueId = dto.PriorityReferenceValueId;
        entity.Reason = dto.Reason;
        entity.EffectiveFrom = dto.EffectiveFrom;
        entity.EffectiveTo = dto.EffectiveTo;
        entity.IsActive = dto.IsActive;

        AuditHelper.ApplyUpdate(entity, _tenant);

        await _repository.UpdateAsync(entity, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return BaseResponse<AppointmentResponseDto>.Ok(_mapper.Map<AppointmentResponseDto>(entity), "Updated.");
    }

    public async Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return BaseResponse<object?>.Fail("Appointment not found.");

        if (!IsAppointmentInFacilityScope(entity))
            return BaseResponse<object?>.Fail("Resource is not in the current facility scope.");

        entity.IsDeleted = true;
        entity.IsActive = false;
        AuditHelper.ApplyUpdate(entity, _tenant);

        await _repository.UpdateAsync(entity, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return BaseResponse<object?>.Ok(null, "Deleted.");
    }

    private static string GenerateNumber(string prefix) =>
        $"{prefix}-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";

    private bool IsAppointmentInFacilityScope(HmsAppointment entity) =>
        _tenant.FacilityId is null || entity.FacilityId == _tenant.FacilityId;
}
