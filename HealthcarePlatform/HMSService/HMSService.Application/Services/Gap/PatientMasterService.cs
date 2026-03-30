using System.Linq;
using AutoMapper;
using FluentValidation;
using Healthcare.Common.Integration.SharedService;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using HMSService.Application.DTOs.Gap;
using HMSService.Domain.Entities;
using HMSService.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace HMSService.Application.Services.Gap;

public sealed class PatientMasterService : IPatientMasterService
{
    private readonly IRepository<HmsPatientMaster> _masters;
    private readonly IRepository<HmsPatientFacilityLink> _links;
    private readonly IMapper _mapper;
    private readonly ITenantContext _tenant;
    private readonly IFacilityTenantValidator _facilityValidator;
    private readonly IValidator<CreatePatientMasterDto> _createValidator;
    private readonly IValidator<UpdatePatientMasterDto> _updateValidator;
    private readonly IValidator<LinkPatientFacilityDto> _linkValidator;
    private readonly ILogger<PatientMasterService> _logger;

    public PatientMasterService(
        IRepository<HmsPatientMaster> masters,
        IRepository<HmsPatientFacilityLink> links,
        IMapper mapper,
        ITenantContext tenant,
        IFacilityTenantValidator facilityValidator,
        IValidator<CreatePatientMasterDto> createValidator,
        IValidator<UpdatePatientMasterDto> updateValidator,
        IValidator<LinkPatientFacilityDto> linkValidator,
        ILogger<PatientMasterService> logger)
    {
        _masters = masters;
        _links = links;
        _mapper = mapper;
        _tenant = tenant;
        _facilityValidator = facilityValidator;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _linkValidator = linkValidator;
        _logger = logger;
    }

    public async Task<BaseResponse<PatientMasterResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _masters.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return BaseResponse<PatientMasterResponseDto>.Fail("Patient master not found.");

        return BaseResponse<PatientMasterResponseDto>.Ok(_mapper.Map<PatientMasterResponseDto>(entity));
    }

    public async Task<BaseResponse<PagedResponse<PatientMasterResponseDto>>> SearchPagedAsync(
        PagedQuery query,
        string? search,
        long? linkedFacilityId,
        CancellationToken cancellationToken = default)
    {
        var searchNorm = string.IsNullOrWhiteSpace(search) ? null : search.Trim();

        if (linkedFacilityId is long fid)
        {
            var ctx = await _facilityValidator.GetFacilityContextAsync(_tenant.TenantId, fid, cancellationToken);
            if (ctx is null)
                return BaseResponse<PagedResponse<PatientMasterResponseDto>>.Fail(
                    "Facility is not valid for this tenant.");

            var masterIds = (await _links.ListAsync(l => l.FacilityId == fid, cancellationToken))
                .Select(l => l.PatientMasterId)
                .Distinct()
                .ToHashSet();

            if (masterIds.Count == 0)
            {
                return BaseResponse<PagedResponse<PatientMasterResponseDto>>.Ok(new PagedResponse<PatientMasterResponseDto>
                {
                    Items = Array.Empty<PatientMasterResponseDto>(),
                    Page = query.Page,
                    PageSize = query.PageSize,
                    TotalCount = 0
                });
            }

            if (searchNorm is null)
            {
                var (items1, total1) = await _masters.GetPagedByFilterAsync(
                    query.Page,
                    query.PageSize,
                    e => masterIds.Contains(e.Id),
                    cancellationToken);
                return BaseResponse<PagedResponse<PatientMasterResponseDto>>.Ok(MapPaged(items1, total1, query));
            }

            var (items2, total2) = await _masters.GetPagedByFilterAsync(
                query.Page,
                query.PageSize,
                e => masterIds.Contains(e.Id) &&
                     (e.FullName.Contains(searchNorm) ||
                      e.Upid.Contains(searchNorm) ||
                      (e.PrimaryPhone != null && e.PrimaryPhone.Contains(searchNorm))),
                cancellationToken);
            return BaseResponse<PagedResponse<PatientMasterResponseDto>>.Ok(MapPaged(items2, total2, query));
        }

        if (searchNorm is null)
        {
            var (items3, total3) = await _masters.GetPagedByFilterAsync(query.Page, query.PageSize, null, cancellationToken);
            return BaseResponse<PagedResponse<PatientMasterResponseDto>>.Ok(MapPaged(items3, total3, query));
        }

        var (items4, total4) = await _masters.GetPagedByFilterAsync(
            query.Page,
            query.PageSize,
            e => e.FullName.Contains(searchNorm) ||
                 e.Upid.Contains(searchNorm) ||
                 (e.PrimaryPhone != null && e.PrimaryPhone.Contains(searchNorm)),
            cancellationToken);
        return BaseResponse<PagedResponse<PatientMasterResponseDto>>.Ok(MapPaged(items4, total4, query));
    }

    public async Task<BaseResponse<PatientMasterResponseDto>> CreateAsync(
        CreatePatientMasterDto dto,
        CancellationToken cancellationToken = default)
    {
        var validation = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return BaseResponse<PatientMasterResponseDto>.Fail(
                "Validation failed.",
                validation.Errors.Select(e => e.ErrorMessage));

        var entity = _mapper.Map<HmsPatientMaster>(dto);
        AuditHelper.ApplyCreate(entity, _tenant);
        entity.FacilityId = null;
        entity.Upid = BuildUpid(_tenant.TenantId);

        await _masters.AddAsync(entity, cancellationToken);
        await _masters.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("HMS PatientMaster created tenant {TenantId} id {Id} upid {Upid}", _tenant.TenantId, entity.Id, entity.Upid);
        return BaseResponse<PatientMasterResponseDto>.Ok(_mapper.Map<PatientMasterResponseDto>(entity), "Created.");
    }

    public async Task<BaseResponse<PatientMasterResponseDto>> UpdateAsync(
        long id,
        UpdatePatientMasterDto dto,
        CancellationToken cancellationToken = default)
    {
        var validation = await _updateValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return BaseResponse<PatientMasterResponseDto>.Fail(
                "Validation failed.",
                validation.Errors.Select(e => e.ErrorMessage));

        var entity = await _masters.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return BaseResponse<PatientMasterResponseDto>.Fail("Patient master not found.");

        _mapper.Map(dto, entity);
        AuditHelper.ApplyUpdate(entity, _tenant);

        await _masters.UpdateAsync(entity, cancellationToken);
        await _masters.SaveChangesAsync(cancellationToken);

        return BaseResponse<PatientMasterResponseDto>.Ok(_mapper.Map<PatientMasterResponseDto>(entity), "Updated.");
    }

    public async Task<BaseResponse<PatientMasterResponseDto>> LinkFacilityAsync(
        LinkPatientFacilityDto dto,
        CancellationToken cancellationToken = default)
    {
        var validation = await _linkValidator.ValidateAsync(dto, cancellationToken);
        if (!validation.IsValid)
            return BaseResponse<PatientMasterResponseDto>.Fail(
                "Validation failed.",
                validation.Errors.Select(e => e.ErrorMessage));

        var master = await _masters.GetByIdAsync(dto.PatientMasterId, cancellationToken);
        if (master is null)
            return BaseResponse<PatientMasterResponseDto>.Fail("Patient master not found.");

        var ctx = await _facilityValidator.GetFacilityContextAsync(_tenant.TenantId, dto.FacilityId, cancellationToken);
        if (ctx is null)
            return BaseResponse<PatientMasterResponseDto>.Fail("Facility is not valid for this tenant.");

        var existing = await _links.ListAsync(
            l => l.PatientMasterId == dto.PatientMasterId && l.FacilityId == dto.FacilityId,
            cancellationToken);
        if (existing.Count > 0)
            return BaseResponse<PatientMasterResponseDto>.Fail("Patient is already linked to this facility.");

        var link = new HmsPatientFacilityLink
        {
            PatientMasterId = dto.PatientMasterId,
            LinkedOn = DateTime.UtcNow,
            Notes = dto.Notes
        };
        AuditHelper.ApplyCreate(link, _tenant);
        link.FacilityId = dto.FacilityId;

        await _links.AddAsync(link, cancellationToken);
        await _links.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "HMS PatientMaster linked to facility master {MasterId} facility {FacilityId}",
            dto.PatientMasterId,
            dto.FacilityId);

        return BaseResponse<PatientMasterResponseDto>.Ok(_mapper.Map<PatientMasterResponseDto>(master), "Linked.");
    }

    private PagedResponse<PatientMasterResponseDto> MapPaged(
        IReadOnlyList<HmsPatientMaster> items,
        int total,
        PagedQuery query) =>
        new()
        {
            Items = _mapper.Map<IReadOnlyList<PatientMasterResponseDto>>(items),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = total
        };

    private static string BuildUpid(long tenantId)
    {
        var suffix = Guid.NewGuid().ToString("N")[..10].ToUpperInvariant();
        return $"UPID-T{tenantId}-{DateTime.UtcNow:yyMMdd}-{suffix}";
    }
}
