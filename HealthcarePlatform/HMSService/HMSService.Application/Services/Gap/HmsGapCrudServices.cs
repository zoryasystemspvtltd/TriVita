using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using FluentValidation;
using Healthcare.Common.Integration.SharedService;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using HMSService.Application.DTOs.Gap;
using HMSService.Application.Services.Extended;
using HMSService.Domain.Entities;
using HMSService.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace HMSService.Application.Services.Gap;

public sealed class WardService
    : HmsCrudServiceBase<HmsWard, CreateWardDto, UpdateWardDto, WardResponseDto, WardService>,
        IWardService
{
    public WardService(
        IRepository<HmsWard> repository,
        IMapper mapper,
        ITenantContext tenant,
        IValidator<CreateWardDto>? createValidator,
        IValidator<UpdateWardDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<WardService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    public Task<BaseResponse<PagedResponse<WardResponseDto>>> GetPagedAsync(
        PagedQuery query,
        CancellationToken cancellationToken = default) =>
        GetPagedCoreAsync(query, null, cancellationToken);
}

public sealed class BedService
    : HmsCrudServiceBase<HmsBed, CreateBedDto, UpdateBedDto, BedResponseDto, BedService>,
        IBedService
{
    private readonly IRepository<HmsWard> _wards;

    public BedService(
        IRepository<HmsBed> repository,
        IRepository<HmsWard> wards,
        IMapper mapper,
        ITenantContext tenant,
        IValidator<CreateBedDto>? createValidator,
        IValidator<UpdateBedDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<BedService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
        _wards = wards;
    }

    public override async Task<BaseResponse<BedResponseDto>> CreateAsync(
        CreateBedDto dto,
        CancellationToken cancellationToken = default)
    {
        if (Tenant.FacilityId is null)
            return BaseResponse<BedResponseDto>.Fail("FacilityId is required (header X-Facility-Id or claim facility_id).");

        var ward = await _wards.GetByIdAsync(dto.WardId, cancellationToken);
        if (ward is null || ward.FacilityId != Tenant.FacilityId)
            return BaseResponse<BedResponseDto>.Fail("Ward not found in the current facility.");

        return await base.CreateAsync(dto, cancellationToken);
    }

    public async Task<BaseResponse<PagedResponse<BedResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? wardId,
        bool? onlyAvailable,
        CancellationToken cancellationToken = default)
    {
        Expression<Func<HmsBed, bool>>? extra = null;
        if (wardId is long w)
            extra = e => e.WardId == w;

        if (onlyAvailable == true)
        {
            Expression<Func<HmsBed, bool>> avail = e => e.CurrentAdmissionId == null;
            extra = extra is null ? avail : AndAlso(extra, avail);
        }

        return await GetPagedCoreAsync(query, extra, cancellationToken);
    }

    private static Expression<Func<HmsBed, bool>> AndAlso(
        Expression<Func<HmsBed, bool>> first,
        Expression<Func<HmsBed, bool>> second)
    {
        var param = Expression.Parameter(typeof(HmsBed), "e");
        var left = new HmsReplaceParameterVisitor(first.Parameters[0], param).Visit(first.Body);
        var right = new HmsReplaceParameterVisitor(second.Parameters[0], param).Visit(second.Body);
        return Expression.Lambda<Func<HmsBed, bool>>(Expression.AndAlso(left!, right!), param);
    }

    private sealed class HmsReplaceParameterVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _from;
        private readonly ParameterExpression _to;

        public HmsReplaceParameterVisitor(ParameterExpression from, ParameterExpression to)
        {
            _from = from;
            _to = to;
        }

        protected override Expression VisitParameter(ParameterExpression node) =>
            node == _from ? _to : base.VisitParameter(node);
    }
}

public sealed class HousekeepingStatusService
    : HmsCrudServiceBase<HmsHousekeepingStatus, CreateHousekeepingStatusDto, UpdateHousekeepingStatusDto, HousekeepingStatusResponseDto, HousekeepingStatusService>,
        IHousekeepingStatusService
{
    public HousekeepingStatusService(
        IRepository<HmsHousekeepingStatus> repository,
        IMapper mapper,
        ITenantContext tenant,
        IValidator<CreateHousekeepingStatusDto>? createValidator,
        IValidator<UpdateHousekeepingStatusDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<HousekeepingStatusService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    public Task<BaseResponse<PagedResponse<HousekeepingStatusResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? bedId,
        CancellationToken cancellationToken = default) =>
        GetPagedCoreAsync(
            query,
            bedId is null ? null : e => e.BedId == bedId.Value,
            cancellationToken);
}

public sealed class EmarEntryService
    : HmsCrudServiceBase<HmsEmarEntry, CreateEmarEntryDto, UpdateEmarEntryDto, EmarEntryResponseDto, EmarEntryService>,
        IEmarEntryService
{
    public EmarEntryService(
        IRepository<HmsEmarEntry> repository,
        IMapper mapper,
        ITenantContext tenant,
        IValidator<CreateEmarEntryDto>? createValidator,
        IValidator<UpdateEmarEntryDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<EmarEntryService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    public Task<BaseResponse<PagedResponse<EmarEntryResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? admissionId,
        CancellationToken cancellationToken = default) =>
        GetPagedCoreAsync(
            query,
            admissionId is null ? null : e => e.AdmissionId == admissionId.Value,
            cancellationToken);
}

public sealed class DoctorOrderAlertService : IDoctorOrderAlertService
{
    private readonly IRepository<HmsDoctorOrderAlert> _repository;
    private readonly IMapper _mapper;
    private readonly ITenantContext _tenant;
    private readonly IFacilityTenantValidator _facilityValidator;
    private readonly IValidator<CreateDoctorOrderAlertDto> _createValidator;
    private readonly IValidator<AcknowledgeDoctorOrderAlertDto> _ackValidator;
    private readonly ILogger<DoctorOrderAlertService> _logger;

    public DoctorOrderAlertService(
        IRepository<HmsDoctorOrderAlert> repository,
        IMapper mapper,
        ITenantContext tenant,
        IFacilityTenantValidator facilityValidator,
        IValidator<CreateDoctorOrderAlertDto> createValidator,
        IValidator<AcknowledgeDoctorOrderAlertDto> ackValidator,
        ILogger<DoctorOrderAlertService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _tenant = tenant;
        _facilityValidator = facilityValidator;
        _createValidator = createValidator;
        _ackValidator = ackValidator;
        _logger = logger;
    }

    public async Task<BaseResponse<DoctorOrderAlertResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null || !IsInFacility(entity))
            return BaseResponse<DoctorOrderAlertResponseDto>.Fail("Doctor order alert not found.");

        return BaseResponse<DoctorOrderAlertResponseDto>.Ok(_mapper.Map<DoctorOrderAlertResponseDto>(entity));
    }

    public async Task<BaseResponse<PagedResponse<DoctorOrderAlertResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? visitId,
        long? admissionId,
        CancellationToken cancellationToken = default)
    {
        if (_tenant.FacilityId is null)
            return BaseResponse<PagedResponse<DoctorOrderAlertResponseDto>>.Fail(
                "FacilityId is required (header X-Facility-Id or claim facility_id).");

        var fid = _tenant.FacilityId.Value;
        Expression<Func<HmsDoctorOrderAlert, bool>> filter =
            visitId is long v && admissionId is long adm
                ? e => e.FacilityId == fid && e.VisitId == v && e.AdmissionId == adm
                : visitId is long vv
                    ? e => e.FacilityId == fid && e.VisitId == vv
                    : admissionId is long aa
                        ? e => e.FacilityId == fid && e.AdmissionId == aa
                        : e => e.FacilityId == fid;

        var (items, total) = await _repository.GetPagedByFilterAsync(query.Page, query.PageSize, filter, cancellationToken);
        var paged = new PagedResponse<DoctorOrderAlertResponseDto>
        {
            Items = _mapper.Map<IReadOnlyList<DoctorOrderAlertResponseDto>>(items),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = total
        };
        return BaseResponse<PagedResponse<DoctorOrderAlertResponseDto>>.Ok(paged);
    }

    public async Task<BaseResponse<DoctorOrderAlertResponseDto>> CreateAsync(
        CreateDoctorOrderAlertDto dto,
        CancellationToken cancellationToken = default)
    {
        if (_tenant.FacilityId is null)
            return BaseResponse<DoctorOrderAlertResponseDto>.Fail(
                "FacilityId is required (header X-Facility-Id or claim facility_id).");

        var v = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!v.IsValid)
            return BaseResponse<DoctorOrderAlertResponseDto>.Fail("Validation failed.", v.Errors.Select(e => e.ErrorMessage));

        var fid = _tenant.FacilityId.Value;
        var ctx = await _facilityValidator.GetFacilityContextAsync(_tenant.TenantId, fid, cancellationToken);
        if (ctx is null)
            return BaseResponse<DoctorOrderAlertResponseDto>.Fail("Facility is not valid for this tenant.");

        var entity = _mapper.Map<HmsDoctorOrderAlert>(dto);
        AuditHelper.ApplyCreate(entity, _tenant);
        entity.FacilityId = fid;

        await _repository.AddAsync(entity, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("HMS DoctorOrderAlert created id {Id}", entity.Id);
        return BaseResponse<DoctorOrderAlertResponseDto>.Ok(_mapper.Map<DoctorOrderAlertResponseDto>(entity), "Created.");
    }

    public async Task<BaseResponse<DoctorOrderAlertResponseDto>> AcknowledgeAsync(
        long id,
        AcknowledgeDoctorOrderAlertDto dto,
        CancellationToken cancellationToken = default)
    {
        var v = await _ackValidator.ValidateAsync(dto, cancellationToken);
        if (!v.IsValid)
            return BaseResponse<DoctorOrderAlertResponseDto>.Fail("Validation failed.", v.Errors.Select(e => e.ErrorMessage));

        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null || !IsInFacility(entity))
            return BaseResponse<DoctorOrderAlertResponseDto>.Fail("Doctor order alert not found.");

        entity.AcknowledgedOn = dto.AcknowledgedOn;
        AuditHelper.ApplyUpdate(entity, _tenant);
        await _repository.UpdateAsync(entity, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return BaseResponse<DoctorOrderAlertResponseDto>.Ok(_mapper.Map<DoctorOrderAlertResponseDto>(entity), "Acknowledged.");
    }

    private bool IsInFacility(HmsDoctorOrderAlert entity) =>
        _tenant.FacilityId is long f && entity.FacilityId == f;
}
