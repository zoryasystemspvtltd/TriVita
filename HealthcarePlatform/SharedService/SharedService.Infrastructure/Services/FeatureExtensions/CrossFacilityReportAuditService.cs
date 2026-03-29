using FluentValidation;
using FluentValidation.Results;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedService.Application.DTOs.FeatureExtensions;
using SharedService.Application.Services.FeatureExtensions;
using SharedService.Domain.FeatureExtensions;
using SharedService.Infrastructure.Persistence;
using SharedService.Infrastructure.Services.Enterprise;

namespace SharedService.Infrastructure.Services.FeatureExtensions;

public sealed class CrossFacilityReportAuditService : ICrossFacilityReportAuditService
{
    private readonly SharedDbContext _db;
    private readonly ITenantContext _tenant;
    private readonly IValidator<CreateCrossFacilityReportAuditDto> _createValidator;
    private readonly IValidator<UpdateCrossFacilityReportAuditDto> _updateValidator;
    private readonly ILogger<CrossFacilityReportAuditService> _logger;

    public CrossFacilityReportAuditService(
        SharedDbContext db,
        ITenantContext tenant,
        IValidator<CreateCrossFacilityReportAuditDto> createValidator,
        IValidator<UpdateCrossFacilityReportAuditDto> updateValidator,
        ILogger<CrossFacilityReportAuditService> logger)
    {
        _db = db;
        _tenant = tenant;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    private long TenantId => _tenant.TenantId;
    private long AuditUser => _tenant.UserId ?? 0;

    public async Task<BaseResponse<CrossFacilityReportAuditResponseDto>> CreateAsync(
        CreateCrossFacilityReportAuditDto dto,
        CancellationToken cancellationToken = default)
    {
        var vr = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!vr.IsValid)
            return FailValidation<CrossFacilityReportAuditResponseDto>(vr);

        if (dto.FacilityId is { } fid &&
            !await EnterpriseReferenceGuard.FacilityExistsAsync(_db, TenantId, fid, cancellationToken))
            return BaseResponse<CrossFacilityReportAuditResponseDto>.Fail("Facility not found.");

        var now = DateTime.UtcNow;
        var entity = new CrossFacilityReportAudit
        {
            TenantId = TenantId,
            FacilityId = dto.FacilityId,
            ReportCode = dto.ReportCode.Trim(),
            ReportName = dto.ReportName?.Trim(),
            FacilityScopeJson = dto.FacilityScopeJson,
            FilterJson = dto.FilterJson,
            IsActive = true,
            IsDeleted = false,
            CreatedOn = now,
            ModifiedOn = now,
            CreatedBy = AuditUser,
            ModifiedBy = AuditUser
        };

        _db.CrossFacilityReportAudits.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Cross-facility report audit created TenantId={TenantId} Id={Id}", TenantId, entity.Id);

        return BaseResponse<CrossFacilityReportAuditResponseDto>.Ok(entity.ToDto(), "Created.");
    }

    public async Task<BaseResponse<CrossFacilityReportAuditResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var row = await _db.CrossFacilityReportAudits.AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id && e.TenantId == TenantId && !e.IsDeleted, cancellationToken);

        return row is null
            ? BaseResponse<CrossFacilityReportAuditResponseDto>.Fail("Report audit not found.")
            : BaseResponse<CrossFacilityReportAuditResponseDto>.Ok(row.ToDto());
    }

    public async Task<BaseResponse<PagedResponse<CrossFacilityReportAuditResponseDto>>> GetPagedAsync(
        PagedQuery query,
        string? reportCode,
        CancellationToken cancellationToken = default)
    {
        var q = _db.CrossFacilityReportAudits.AsNoTracking()
            .Where(e => e.TenantId == TenantId && !e.IsDeleted);

        if (!string.IsNullOrWhiteSpace(reportCode))
        {
            var rc = reportCode.Trim();
            q = q.Where(e => e.ReportCode == rc);
        }

        var total = await q.CountAsync(cancellationToken);
        var rows = await q.OrderByDescending(e => e.CreatedOn)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return BaseResponse<PagedResponse<CrossFacilityReportAuditResponseDto>>.Ok(new PagedResponse<CrossFacilityReportAuditResponseDto>
        {
            Items = rows.Select(e => e.ToDto()).ToList(),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = total
        });
    }

    public async Task<BaseResponse<CrossFacilityReportAuditResponseDto>> UpdateAsync(
        long id,
        UpdateCrossFacilityReportAuditDto dto,
        CancellationToken cancellationToken = default)
    {
        var vr = await _updateValidator.ValidateAsync(dto, cancellationToken);
        if (!vr.IsValid)
            return FailValidation<CrossFacilityReportAuditResponseDto>(vr);

        var entity = await _db.CrossFacilityReportAudits.FirstOrDefaultAsync(
            e => e.Id == id && e.TenantId == TenantId && !e.IsDeleted,
            cancellationToken);

        if (entity is null)
            return BaseResponse<CrossFacilityReportAuditResponseDto>.Fail("Report audit not found.");

        if (dto.ReportName is not null)
            entity.ReportName = dto.ReportName.Trim();
        entity.ResultRowCount = dto.ResultRowCount;
        entity.CompletedOn = dto.CompletedOn;
        entity.IsActive = dto.IsActive;
        entity.ModifiedOn = DateTime.UtcNow;
        entity.ModifiedBy = AuditUser;

        await _db.SaveChangesAsync(cancellationToken);

        return BaseResponse<CrossFacilityReportAuditResponseDto>.Ok(entity.ToDto(), "Updated.");
    }

    public async Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.CrossFacilityReportAudits.FirstOrDefaultAsync(
            e => e.Id == id && e.TenantId == TenantId && !e.IsDeleted,
            cancellationToken);

        if (entity is null)
            return BaseResponse<object?>.Fail("Report audit not found.");

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
