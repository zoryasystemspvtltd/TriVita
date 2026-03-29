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

public sealed class ModuleIntegrationHandoffService : IModuleIntegrationHandoffService
{
    private readonly SharedDbContext _db;
    private readonly ITenantContext _tenant;
    private readonly IValidator<CreateModuleIntegrationHandoffDto> _createValidator;
    private readonly IValidator<UpdateModuleIntegrationHandoffDto> _updateValidator;
    private readonly ILogger<ModuleIntegrationHandoffService> _logger;

    public ModuleIntegrationHandoffService(
        SharedDbContext db,
        ITenantContext tenant,
        IValidator<CreateModuleIntegrationHandoffDto> createValidator,
        IValidator<UpdateModuleIntegrationHandoffDto> updateValidator,
        ILogger<ModuleIntegrationHandoffService> logger)
    {
        _db = db;
        _tenant = tenant;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    private long TenantId => _tenant.TenantId;
    private long AuditUser => _tenant.UserId ?? 0;

    public async Task<BaseResponse<ModuleIntegrationHandoffResponseDto>> CreateAsync(
        CreateModuleIntegrationHandoffDto dto,
        CancellationToken cancellationToken = default)
    {
        var vr = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!vr.IsValid)
            return FailValidation<ModuleIntegrationHandoffResponseDto>(vr);

        if (dto.FacilityId is { } fid &&
            !await EnterpriseReferenceGuard.FacilityExistsAsync(_db, TenantId, fid, cancellationToken))
            return BaseResponse<ModuleIntegrationHandoffResponseDto>.Fail("Facility not found.");

        var now = DateTime.UtcNow;
        var entity = new ModuleIntegrationHandoff
        {
            TenantId = TenantId,
            FacilityId = dto.FacilityId,
            CorrelationId = dto.CorrelationId.Trim(),
            SourceModule = dto.SourceModule.Trim(),
            TargetModule = dto.TargetModule.Trim(),
            EntityType = dto.EntityType.Trim(),
            SourceEntityId = dto.SourceEntityId,
            TargetEntityId = null,
            StatusCode = dto.StatusCode.Trim(),
            DetailJson = dto.DetailJson,
            IsActive = true,
            IsDeleted = false,
            CreatedOn = now,
            ModifiedOn = now,
            CreatedBy = AuditUser,
            ModifiedBy = AuditUser
        };

        _db.ModuleIntegrationHandoffs.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Module handoff created TenantId={TenantId} Id={Id} Correlation={C}", TenantId, entity.Id, entity.CorrelationId);

        return BaseResponse<ModuleIntegrationHandoffResponseDto>.Ok(entity.ToDto(), "Created.");
    }

    public async Task<BaseResponse<ModuleIntegrationHandoffResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var row = await _db.ModuleIntegrationHandoffs.AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id && e.TenantId == TenantId && !e.IsDeleted, cancellationToken);

        return row is null
            ? BaseResponse<ModuleIntegrationHandoffResponseDto>.Fail("Handoff not found.")
            : BaseResponse<ModuleIntegrationHandoffResponseDto>.Ok(row.ToDto());
    }

    public async Task<BaseResponse<IReadOnlyList<ModuleIntegrationHandoffResponseDto>>> ListByCorrelationAsync(
        string correlationId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(correlationId))
            return BaseResponse<IReadOnlyList<ModuleIntegrationHandoffResponseDto>>.Fail("Correlation id is required.");

        var c = correlationId.Trim();
        var rows = await _db.ModuleIntegrationHandoffs.AsNoTracking()
            .Where(e => e.TenantId == TenantId && e.CorrelationId == c && !e.IsDeleted)
            .OrderBy(e => e.CreatedOn)
            .ToListAsync(cancellationToken);

        return BaseResponse<IReadOnlyList<ModuleIntegrationHandoffResponseDto>>.Ok(rows.Select(e => e.ToDto()).ToList());
    }

    public async Task<BaseResponse<ModuleIntegrationHandoffResponseDto>> UpdateAsync(
        long id,
        UpdateModuleIntegrationHandoffDto dto,
        CancellationToken cancellationToken = default)
    {
        var vr = await _updateValidator.ValidateAsync(dto, cancellationToken);
        if (!vr.IsValid)
            return FailValidation<ModuleIntegrationHandoffResponseDto>(vr);

        var entity = await _db.ModuleIntegrationHandoffs.FirstOrDefaultAsync(
            e => e.Id == id && e.TenantId == TenantId && !e.IsDeleted,
            cancellationToken);

        if (entity is null)
            return BaseResponse<ModuleIntegrationHandoffResponseDto>.Fail("Handoff not found.");

        entity.TargetEntityId = dto.TargetEntityId;
        entity.StatusCode = dto.StatusCode.Trim();
        entity.DetailJson = dto.DetailJson;
        entity.IsActive = dto.IsActive;
        entity.ModifiedOn = DateTime.UtcNow;
        entity.ModifiedBy = AuditUser;

        await _db.SaveChangesAsync(cancellationToken);

        return BaseResponse<ModuleIntegrationHandoffResponseDto>.Ok(entity.ToDto(), "Updated.");
    }

    public async Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.ModuleIntegrationHandoffs.FirstOrDefaultAsync(
            e => e.Id == id && e.TenantId == TenantId && !e.IsDeleted,
            cancellationToken);

        if (entity is null)
            return BaseResponse<object?>.Fail("Handoff not found.");

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
