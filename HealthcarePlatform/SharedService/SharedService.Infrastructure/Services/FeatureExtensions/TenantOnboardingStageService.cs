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

public sealed class TenantOnboardingStageService : ITenantOnboardingStageService
{
    private readonly SharedDbContext _db;
    private readonly ITenantContext _tenant;
    private readonly IValidator<UpsertTenantOnboardingStageDto> _upsertValidator;
    private readonly ILogger<TenantOnboardingStageService> _logger;

    public TenantOnboardingStageService(
        SharedDbContext db,
        ITenantContext tenant,
        IValidator<UpsertTenantOnboardingStageDto> upsertValidator,
        ILogger<TenantOnboardingStageService> logger)
    {
        _db = db;
        _tenant = tenant;
        _upsertValidator = upsertValidator;
        _logger = logger;
    }

    private long TenantId => _tenant.TenantId;
    private long AuditUser => _tenant.UserId ?? 0;

    public async Task<BaseResponse<IReadOnlyList<TenantOnboardingStageResponseDto>>> ListAsync(CancellationToken cancellationToken = default)
    {
        var rows = await _db.TenantOnboardingStages.AsNoTracking()
            .Where(e => e.TenantId == TenantId && !e.IsDeleted)
            .OrderBy(e => e.StageCode)
            .ToListAsync(cancellationToken);

        return BaseResponse<IReadOnlyList<TenantOnboardingStageResponseDto>>.Ok(rows.Select(e => e.ToDto()).ToList());
    }

    public async Task<BaseResponse<TenantOnboardingStageResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var row = await _db.TenantOnboardingStages.AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id && e.TenantId == TenantId && !e.IsDeleted, cancellationToken);

        return row is null
            ? BaseResponse<TenantOnboardingStageResponseDto>.Fail("Onboarding stage not found.")
            : BaseResponse<TenantOnboardingStageResponseDto>.Ok(row.ToDto());
    }

    public async Task<BaseResponse<TenantOnboardingStageResponseDto>> GetByStageCodeAsync(
        string stageCode,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(stageCode))
            return BaseResponse<TenantOnboardingStageResponseDto>.Fail("Stage code is required.");

        var sc = stageCode.Trim();
        var row = await _db.TenantOnboardingStages.AsNoTracking()
            .FirstOrDefaultAsync(e => e.TenantId == TenantId && e.StageCode == sc && !e.IsDeleted, cancellationToken);

        return row is null
            ? BaseResponse<TenantOnboardingStageResponseDto>.Fail("Onboarding stage not found.")
            : BaseResponse<TenantOnboardingStageResponseDto>.Ok(row.ToDto());
    }

    public async Task<BaseResponse<TenantOnboardingStageResponseDto>> UpsertAsync(
        UpsertTenantOnboardingStageDto dto,
        CancellationToken cancellationToken = default)
    {
        var vr = await _upsertValidator.ValidateAsync(dto, cancellationToken);
        if (!vr.IsValid)
            return FailValidation<TenantOnboardingStageResponseDto>(vr);

        if (dto.FacilityId is { } fid &&
            !await EnterpriseReferenceGuard.FacilityExistsAsync(_db, TenantId, fid, cancellationToken))
            return BaseResponse<TenantOnboardingStageResponseDto>.Fail("Facility not found.");

        var code = dto.StageCode.Trim();
        var existing = await _db.TenantOnboardingStages.FirstOrDefaultAsync(
            e => e.TenantId == TenantId && e.StageCode == code && !e.IsDeleted,
            cancellationToken);

        var now = DateTime.UtcNow;
        if (existing is null)
        {
            var entity = new TenantOnboardingStage
            {
                TenantId = TenantId,
                FacilityId = dto.FacilityId,
                StageCode = code,
                StageStatus = dto.StageStatus.Trim(),
                CompletedOn = dto.CompletedOn,
                MetadataJson = dto.MetadataJson,
                IsActive = dto.IsActive,
                IsDeleted = false,
                CreatedOn = now,
                ModifiedOn = now,
                CreatedBy = AuditUser,
                ModifiedBy = AuditUser
            };
            _db.TenantOnboardingStages.Add(entity);
            await _db.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Onboarding stage created TenantId={TenantId} Code={Code}", TenantId, code);
            return BaseResponse<TenantOnboardingStageResponseDto>.Ok(entity.ToDto(), "Created.");
        }

        existing.FacilityId = dto.FacilityId;
        existing.StageStatus = dto.StageStatus.Trim();
        existing.CompletedOn = dto.CompletedOn;
        existing.MetadataJson = dto.MetadataJson;
        existing.IsActive = dto.IsActive;
        existing.ModifiedOn = now;
        existing.ModifiedBy = AuditUser;

        await _db.SaveChangesAsync(cancellationToken);

        return BaseResponse<TenantOnboardingStageResponseDto>.Ok(existing.ToDto(), "Updated.");
    }

    public async Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.TenantOnboardingStages.FirstOrDefaultAsync(
            e => e.Id == id && e.TenantId == TenantId && !e.IsDeleted,
            cancellationToken);

        if (entity is null)
            return BaseResponse<object?>.Fail("Onboarding stage not found.");

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
