using FluentValidation;
using FluentValidation.Results;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedService.Application.DTOs.FeatureExtensions;
using SharedService.Application.Services.FeatureExtensions;
using SharedService.Domain.FeatureExtensions;
using SharedService.Infrastructure.Persistence;
using SharedService.Infrastructure.Services.Enterprise;

namespace SharedService.Infrastructure.Services.FeatureExtensions;

public sealed class LabCriticalValueEscalationService : ILabCriticalValueEscalationService
{
    private readonly SharedDbContext _db;
    private readonly ITenantContext _tenant;
    private readonly IValidator<CreateLabCriticalValueEscalationDto> _createValidator;
    private readonly IValidator<UpdateLabCriticalValueEscalationDto> _updateValidator;
    private readonly ILogger<LabCriticalValueEscalationService> _logger;

    public LabCriticalValueEscalationService(
        SharedDbContext db,
        ITenantContext tenant,
        IValidator<CreateLabCriticalValueEscalationDto> createValidator,
        IValidator<UpdateLabCriticalValueEscalationDto> updateValidator,
        ILogger<LabCriticalValueEscalationService> logger)
    {
        _db = db;
        _tenant = tenant;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    private long TenantId => _tenant.TenantId;
    private long AuditUser => _tenant.UserId ?? 0;

    public async Task<BaseResponse<LabCriticalValueEscalationResponseDto>> CreateAsync(
        CreateLabCriticalValueEscalationDto dto,
        CancellationToken cancellationToken = default)
    {
        var vr = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!vr.IsValid)
            return FailValidation<LabCriticalValueEscalationResponseDto>(vr);

        if (!await EnterpriseReferenceGuard.FacilityExistsAsync(_db, TenantId, dto.FacilityId, cancellationToken))
            return BaseResponse<LabCriticalValueEscalationResponseDto>.Fail("Facility not found.");

        var now = DateTime.UtcNow;
        var entity = new LabCriticalValueEscalation
        {
            TenantId = TenantId,
            FacilityId = dto.FacilityId,
            LabOrderId = dto.LabOrderId,
            LabOrderItemId = dto.LabOrderItemId,
            LabResultId = dto.LabResultId,
            EscalationLevel = dto.EscalationLevel,
            ChannelCode = dto.ChannelCode.Trim(),
            RecipientSummary = dto.RecipientSummary?.Trim(),
            DispatchedOn = dto.DispatchedOn,
            AcknowledgedOn = null,
            OutcomeCode = null,
            IsActive = true,
            IsDeleted = false,
            CreatedOn = now,
            ModifiedOn = now,
            CreatedBy = AuditUser,
            ModifiedBy = AuditUser
        };

        _db.LabCriticalValueEscalations.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Lab critical escalation created TenantId={TenantId} Id={Id} FacilityId={Fid}",
            TenantId,
            entity.Id,
            entity.FacilityId);

        return BaseResponse<LabCriticalValueEscalationResponseDto>.Ok(entity.ToDto(), "Created.");
    }

    public async Task<BaseResponse<LabCriticalValueEscalationResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var row = await _db.LabCriticalValueEscalations.AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id && e.TenantId == TenantId && !e.IsDeleted, cancellationToken);

        return row is null
            ? BaseResponse<LabCriticalValueEscalationResponseDto>.Fail("Escalation not found.")
            : BaseResponse<LabCriticalValueEscalationResponseDto>.Ok(row.ToDto());
    }

    public async Task<BaseResponse<IReadOnlyList<LabCriticalValueEscalationResponseDto>>> ListByLabResultAsync(
        long facilityId,
        long labResultId,
        CancellationToken cancellationToken = default)
    {
        if (!await EnterpriseReferenceGuard.FacilityExistsAsync(_db, TenantId, facilityId, cancellationToken))
            return BaseResponse<IReadOnlyList<LabCriticalValueEscalationResponseDto>>.Fail("Facility not found.");

        var rows = await _db.LabCriticalValueEscalations.AsNoTracking()
            .Where(e => e.TenantId == TenantId && e.FacilityId == facilityId && e.LabResultId == labResultId && !e.IsDeleted)
            .OrderByDescending(e => e.CreatedOn)
            .ToListAsync(cancellationToken);

        return BaseResponse<IReadOnlyList<LabCriticalValueEscalationResponseDto>>.Ok(rows.Select(e => e.ToDto()).ToList());
    }

    public async Task<BaseResponse<LabCriticalValueEscalationResponseDto>> UpdateAsync(
        long id,
        UpdateLabCriticalValueEscalationDto dto,
        CancellationToken cancellationToken = default)
    {
        var vr = await _updateValidator.ValidateAsync(dto, cancellationToken);
        if (!vr.IsValid)
            return FailValidation<LabCriticalValueEscalationResponseDto>(vr);

        var entity = await _db.LabCriticalValueEscalations.FirstOrDefaultAsync(
            e => e.Id == id && e.TenantId == TenantId && !e.IsDeleted,
            cancellationToken);

        if (entity is null)
            return BaseResponse<LabCriticalValueEscalationResponseDto>.Fail("Escalation not found.");

        entity.DispatchedOn = dto.DispatchedOn;
        entity.AcknowledgedOn = dto.AcknowledgedOn;
        entity.OutcomeCode = dto.OutcomeCode?.Trim();
        entity.IsActive = dto.IsActive;
        entity.ModifiedOn = DateTime.UtcNow;
        entity.ModifiedBy = AuditUser;

        await _db.SaveChangesAsync(cancellationToken);

        return BaseResponse<LabCriticalValueEscalationResponseDto>.Ok(entity.ToDto(), "Updated.");
    }

    public async Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.LabCriticalValueEscalations.FirstOrDefaultAsync(
            e => e.Id == id && e.TenantId == TenantId && !e.IsDeleted,
            cancellationToken);

        if (entity is null)
            return BaseResponse<object?>.Fail("Escalation not found.");

        entity.IsDeleted = true;
        entity.IsActive = false;
        entity.ModifiedOn = DateTime.UtcNow;
        entity.ModifiedBy = AuditUser;

        await _db.SaveChangesAsync(cancellationToken);

        return BaseResponse<object?>.Ok(null, "Deleted.");
    }

    private static BaseResponse<T> FailValidation<T>(ValidationResult vr) =>
        BaseResponse<T>.Fail(string.Join(" ", vr.Errors.Select(e => e.ErrorMessage)));
}
