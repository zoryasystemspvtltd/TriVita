using AutoMapper;
using FluentValidation;
using Healthcare.Common.Integration.SharedService;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using HMSService.Application.DTOs.Visits;
using HMSService.Domain.Entities;
using HMSService.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace HMSService.Application.Services;

public sealed class VisitService : IVisitService
{
    private readonly IVisitRepository _visitRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IMapper _mapper;
    private readonly ITenantContext _tenant;
    private readonly IValidator<CreateVisitDto> _createValidator;
    private readonly IValidator<UpdateVisitDto> _updateValidator;
    private readonly IFacilityTenantValidator _facilityValidator;
    private readonly ILogger<VisitService> _logger;

    public VisitService(
        IVisitRepository visitRepository,
        IAppointmentRepository appointmentRepository,
        IMapper mapper,
        ITenantContext tenant,
        IValidator<CreateVisitDto> createValidator,
        IValidator<UpdateVisitDto> updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<VisitService> logger)
    {
        _visitRepository = visitRepository;
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
        _tenant = tenant;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _facilityValidator = facilityValidator;
        _logger = logger;
    }

    public async Task<BaseResponse<VisitResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _visitRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return BaseResponse<VisitResponseDto>.Fail("Visit not found.");

        if (!IsVisitInFacilityScope(entity))
            return BaseResponse<VisitResponseDto>.Fail("Resource is not in the current facility scope.");

        return BaseResponse<VisitResponseDto>.Ok(_mapper.Map<VisitResponseDto>(entity));
    }

    public async Task<BaseResponse<PagedResponse<VisitResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? patientId,
        long? doctorId,
        DateTime? visitFrom,
        DateTime? visitTo,
        CancellationToken cancellationToken = default)
    {
        var (items, total) = await _visitRepository.GetPagedAsync(
            query.Page,
            query.PageSize,
            query.SortBy,
            query.SortDescending,
            patientId,
            doctorId,
            visitFrom,
            visitTo,
            _tenant.FacilityId,
            cancellationToken);

        var dtoItems = _mapper.Map<IReadOnlyList<VisitResponseDto>>(items);

        var paged = new PagedResponse<VisitResponseDto>
        {
            Items = dtoItems,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = total
        };

        return BaseResponse<PagedResponse<VisitResponseDto>>.Ok(paged);
    }

    public async Task<BaseResponse<VisitResponseDto>> CreateAsync(
        CreateVisitDto dto,
        CancellationToken cancellationToken = default)
    {
        if (_tenant.FacilityId is null)
            return BaseResponse<VisitResponseDto>.Fail("FacilityId is required (header X-Facility-Id or claim facility_id).");

        if (await _facilityValidator.GetFacilityContextAsync(_tenant.TenantId, _tenant.FacilityId.Value, cancellationToken) is null)
        {
            _logger.LogWarning(
                "Visit create blocked: invalid facility TenantId={TenantId} FacilityId={FacilityId}",
                _tenant.TenantId,
                _tenant.FacilityId);
            return BaseResponse<VisitResponseDto>.Fail(
                "Facility is not valid for this tenant (shared enterprise hierarchy).");
        }

        var validation = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return BaseResponse<VisitResponseDto>.Fail(
                "Validation failed.",
                validation.Errors.Select(e => e.ErrorMessage));

        if (dto.AppointmentId is not null)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(dto.AppointmentId.Value, cancellationToken);
            if (appointment is null)
                return BaseResponse<VisitResponseDto>.Fail("Linked appointment was not found.");

            if (appointment.FacilityId != _tenant.FacilityId)
                return BaseResponse<VisitResponseDto>.Fail("Appointment belongs to a different facility.");
        }

        var entity = _mapper.Map<HmsVisit>(dto);
        AuditHelper.ApplyCreate(entity, _tenant);
        entity.FacilityId = _tenant.FacilityId;
        entity.VisitNo = GenerateNumber("VIS");

        await _visitRepository.AddAsync(entity, cancellationToken);
        await _visitRepository.SaveChangesAsync(cancellationToken);

        return BaseResponse<VisitResponseDto>.Ok(_mapper.Map<VisitResponseDto>(entity), "Created.");
    }

    public async Task<BaseResponse<VisitResponseDto>> UpdateAsync(
        long id,
        UpdateVisitDto dto,
        CancellationToken cancellationToken = default)
    {
        var validation = await _updateValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return BaseResponse<VisitResponseDto>.Fail(
                "Validation failed.",
                validation.Errors.Select(e => e.ErrorMessage));

        var entity = await _visitRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return BaseResponse<VisitResponseDto>.Fail("Visit not found.");

        if (!IsVisitInFacilityScope(entity))
            return BaseResponse<VisitResponseDto>.Fail("Resource is not in the current facility scope.");

        entity.DoctorId = dto.DoctorId;
        entity.DepartmentId = dto.DepartmentId;
        entity.VisitTypeId = dto.VisitTypeId;
        entity.VisitStartOn = dto.VisitStartOn;
        entity.VisitEndOn = dto.VisitEndOn;
        entity.ChiefComplaint = dto.ChiefComplaint;
        entity.CurrentStatusReferenceValueId = dto.CurrentStatusReferenceValueId;
        entity.IsActive = dto.IsActive;

        AuditHelper.ApplyUpdate(entity, _tenant);

        await _visitRepository.UpdateAsync(entity, cancellationToken);
        await _visitRepository.SaveChangesAsync(cancellationToken);

        return BaseResponse<VisitResponseDto>.Ok(_mapper.Map<VisitResponseDto>(entity), "Updated.");
    }

    public async Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _visitRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return BaseResponse<object?>.Fail("Visit not found.");

        if (!IsVisitInFacilityScope(entity))
            return BaseResponse<object?>.Fail("Resource is not in the current facility scope.");

        entity.IsDeleted = true;
        entity.IsActive = false;
        AuditHelper.ApplyUpdate(entity, _tenant);

        await _visitRepository.UpdateAsync(entity, cancellationToken);
        await _visitRepository.SaveChangesAsync(cancellationToken);

        return BaseResponse<object?>.Ok(null, "Deleted.");
    }

    private static string GenerateNumber(string prefix) =>
        $"{prefix}-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";

    private bool IsVisitInFacilityScope(HmsVisit entity) =>
        _tenant.FacilityId is null || entity.FacilityId == _tenant.FacilityId;
}
