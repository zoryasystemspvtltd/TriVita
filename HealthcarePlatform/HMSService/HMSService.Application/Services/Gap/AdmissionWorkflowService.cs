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

public sealed class AdmissionWorkflowService : IAdmissionWorkflowService
{
    private readonly IRepository<HmsAdmission> _admissions;
    private readonly IRepository<HmsBed> _beds;
    private readonly IRepository<HmsAdmissionTransfer> _transfers;
    private readonly IRepository<HmsPatientMaster> _masters;
    private readonly IMapper _mapper;
    private readonly ITenantContext _tenant;
    private readonly IFacilityTenantValidator _facilityValidator;
    private readonly IValidator<AdmitPatientDto> _admitValidator;
    private readonly IValidator<TransferPatientDto> _transferValidator;
    private readonly IValidator<DischargePatientDto> _dischargeValidator;
    private readonly ILogger<AdmissionWorkflowService> _logger;

    public AdmissionWorkflowService(
        IRepository<HmsAdmission> admissions,
        IRepository<HmsBed> beds,
        IRepository<HmsAdmissionTransfer> transfers,
        IRepository<HmsPatientMaster> masters,
        IMapper mapper,
        ITenantContext tenant,
        IFacilityTenantValidator facilityValidator,
        IValidator<AdmitPatientDto> admitValidator,
        IValidator<TransferPatientDto> transferValidator,
        IValidator<DischargePatientDto> dischargeValidator,
        ILogger<AdmissionWorkflowService> logger)
    {
        _admissions = admissions;
        _beds = beds;
        _transfers = transfers;
        _masters = masters;
        _mapper = mapper;
        _tenant = tenant;
        _facilityValidator = facilityValidator;
        _admitValidator = admitValidator;
        _transferValidator = transferValidator;
        _dischargeValidator = dischargeValidator;
        _logger = logger;
    }

    public async Task<BaseResponse<AdmissionResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _admissions.GetByIdAsync(id, cancellationToken);
        if (entity is null || !IsInCurrentFacility(entity))
            return BaseResponse<AdmissionResponseDto>.Fail("Admission not found.");

        return BaseResponse<AdmissionResponseDto>.Ok(_mapper.Map<AdmissionResponseDto>(entity));
    }

    public Task<BaseResponse<PagedResponse<AdmissionResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? patientMasterId,
        CancellationToken cancellationToken = default)
    {
        if (_tenant.FacilityId is null)
            return Task.FromResult(
                BaseResponse<PagedResponse<AdmissionResponseDto>>.Fail(
                    "FacilityId is required (header X-Facility-Id or claim facility_id)."));

        var fid = _tenant.FacilityId.Value;
        return GetPagedCoreAsync(query, patientMasterId, fid, cancellationToken);
    }

    private async Task<BaseResponse<PagedResponse<AdmissionResponseDto>>> GetPagedCoreAsync(
        PagedQuery query,
        long? patientMasterId,
        long facilityId,
        CancellationToken cancellationToken)
    {
        var (items, total) = await _admissions.GetPagedByFilterAsync(
            query.Page,
            query.PageSize,
            patientMasterId is null
                ? e => e.FacilityId == facilityId
                : e => e.FacilityId == facilityId && e.PatientMasterId == patientMasterId.Value,
            cancellationToken);

        var paged = new PagedResponse<AdmissionResponseDto>
        {
            Items = _mapper.Map<IReadOnlyList<AdmissionResponseDto>>(items),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = total
        };

        return BaseResponse<PagedResponse<AdmissionResponseDto>>.Ok(paged);
    }

    public async Task<BaseResponse<AdmissionResponseDto>> AdmitAsync(AdmitPatientDto dto, CancellationToken cancellationToken = default)
    {
        if (_tenant.FacilityId is null)
            return BaseResponse<AdmissionResponseDto>.Fail(
                "FacilityId is required (header X-Facility-Id or claim facility_id).");

        var v = await _admitValidator.ValidateAsync(dto, cancellationToken);
        if (!v.IsValid)
            return BaseResponse<AdmissionResponseDto>.Fail("Validation failed.", v.Errors.Select(e => e.ErrorMessage));

        var fid = _tenant.FacilityId.Value;
        var ctx = await _facilityValidator.GetFacilityContextAsync(_tenant.TenantId, fid, cancellationToken);
        if (ctx is null)
            return BaseResponse<AdmissionResponseDto>.Fail("Facility is not valid for this tenant.");

        var master = await _masters.GetByIdAsync(dto.PatientMasterId, cancellationToken);
        if (master is null)
            return BaseResponse<AdmissionResponseDto>.Fail("Patient master not found.");

        var bed = await _beds.GetByIdAsync(dto.BedId, cancellationToken);
        if (bed is null || bed.FacilityId != fid)
            return BaseResponse<AdmissionResponseDto>.Fail("Bed not found in the current facility.");

        var activeOnBed = await _admissions.ListAsync(
            a => a.BedId == bed.Id && a.DischargedOn == null,
            cancellationToken);
        if (activeOnBed.Count > 0)
            return BaseResponse<AdmissionResponseDto>.Fail("Bed is already occupied.");

        if (bed.CurrentAdmissionId is not null)
            return BaseResponse<AdmissionResponseDto>.Fail("Bed current admission pointer is set; resolve before admitting.");

        var admission = new HmsAdmission
        {
            AdmissionNo = BuildAdmissionNo(fid),
            PatientMasterId = dto.PatientMasterId,
            BedId = dto.BedId,
            AdmissionStatusReferenceValueId = dto.AdmissionStatusReferenceValueId,
            AdmittedOn = DateTime.UtcNow,
            AttendingDoctorId = dto.AttendingDoctorId
        };
        AuditHelper.ApplyCreate(admission, _tenant);
        admission.FacilityId = fid;

        await _admissions.AddAsync(admission, cancellationToken);
        await _admissions.SaveChangesAsync(cancellationToken);

        bed.CurrentAdmissionId = admission.Id;
        AuditHelper.ApplyUpdate(bed, _tenant);
        await _beds.UpdateAsync(bed, cancellationToken);
        await _beds.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("HMS Admission created id {Id} bed {BedId}", admission.Id, bed.Id);
        return BaseResponse<AdmissionResponseDto>.Ok(_mapper.Map<AdmissionResponseDto>(admission), "Admitted.");
    }

    public async Task<BaseResponse<AdmissionResponseDto>> TransferAsync(TransferPatientDto dto, CancellationToken cancellationToken = default)
    {
        if (_tenant.FacilityId is null)
            return BaseResponse<AdmissionResponseDto>.Fail(
                "FacilityId is required (header X-Facility-Id or claim facility_id).");

        var v = await _transferValidator.ValidateAsync(dto, cancellationToken);
        if (!v.IsValid)
            return BaseResponse<AdmissionResponseDto>.Fail("Validation failed.", v.Errors.Select(e => e.ErrorMessage));

        var fid = _tenant.FacilityId.Value;

        var admission = await _admissions.GetByIdAsync(dto.AdmissionId, cancellationToken);
        if (admission is null || !IsInCurrentFacility(admission))
            return BaseResponse<AdmissionResponseDto>.Fail("Admission not found.");
        if (admission.DischargedOn is not null)
            return BaseResponse<AdmissionResponseDto>.Fail("Cannot transfer a discharged admission.");

        var fromBed = await _beds.GetByIdAsync(admission.BedId, cancellationToken);
        if (fromBed is null || fromBed.FacilityId != fid)
            return BaseResponse<AdmissionResponseDto>.Fail("Source bed not found.");

        var toBed = await _beds.GetByIdAsync(dto.ToBedId, cancellationToken);
        if (toBed is null || toBed.FacilityId != fid)
            return BaseResponse<AdmissionResponseDto>.Fail("Target bed not found in the current facility.");

        if (toBed.Id == fromBed.Id)
            return BaseResponse<AdmissionResponseDto>.Fail("Target bed must differ from the current bed.");

        var activeOnTarget = await _admissions.ListAsync(
            a => a.BedId == toBed.Id && a.DischargedOn == null && a.Id != admission.Id,
            cancellationToken);
        if (activeOnTarget.Count > 0)
            return BaseResponse<AdmissionResponseDto>.Fail("Target bed is already occupied.");

        if (toBed.CurrentAdmissionId is not null && toBed.CurrentAdmissionId != admission.Id)
            return BaseResponse<AdmissionResponseDto>.Fail("Target bed is not available.");

        var transfer = new HmsAdmissionTransfer
        {
            AdmissionId = admission.Id,
            FromBedId = fromBed.Id,
            ToBedId = toBed.Id,
            TransferredOn = DateTime.UtcNow,
            Reason = dto.Reason
        };
        AuditHelper.ApplyCreate(transfer, _tenant);
        transfer.FacilityId = fid;

        fromBed.CurrentAdmissionId = null;
        AuditHelper.ApplyUpdate(fromBed, _tenant);
        toBed.CurrentAdmissionId = admission.Id;
        AuditHelper.ApplyUpdate(toBed, _tenant);
        admission.BedId = toBed.Id;
        AuditHelper.ApplyUpdate(admission, _tenant);

        await _transfers.AddAsync(transfer, cancellationToken);
        await _beds.UpdateAsync(fromBed, cancellationToken);
        await _beds.UpdateAsync(toBed, cancellationToken);
        await _admissions.UpdateAsync(admission, cancellationToken);
        await _admissions.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("HMS Admission {Id} transferred to bed {BedId}", admission.Id, toBed.Id);
        return BaseResponse<AdmissionResponseDto>.Ok(_mapper.Map<AdmissionResponseDto>(admission), "Transferred.");
    }

    public async Task<BaseResponse<AdmissionResponseDto>> DischargeAsync(DischargePatientDto dto, CancellationToken cancellationToken = default)
    {
        if (_tenant.FacilityId is null)
            return BaseResponse<AdmissionResponseDto>.Fail(
                "FacilityId is required (header X-Facility-Id or claim facility_id).");

        var v = await _dischargeValidator.ValidateAsync(dto, cancellationToken);
        if (!v.IsValid)
            return BaseResponse<AdmissionResponseDto>.Fail("Validation failed.", v.Errors.Select(e => e.ErrorMessage));

        var fid = _tenant.FacilityId.Value;

        var admission = await _admissions.GetByIdAsync(dto.AdmissionId, cancellationToken);
        if (admission is null || !IsInCurrentFacility(admission))
            return BaseResponse<AdmissionResponseDto>.Fail("Admission not found.");
        if (admission.DischargedOn is not null)
            return BaseResponse<AdmissionResponseDto>.Fail("Admission is already discharged.");

        var bed = await _beds.GetByIdAsync(admission.BedId, cancellationToken);
        if (bed is not null && bed.FacilityId == fid && bed.CurrentAdmissionId == admission.Id)
        {
            bed.CurrentAdmissionId = null;
            AuditHelper.ApplyUpdate(bed, _tenant);
            await _beds.UpdateAsync(bed, cancellationToken);
        }

        admission.DischargedOn = DateTime.UtcNow;
        admission.AdmissionStatusReferenceValueId = dto.AdmissionStatusReferenceValueId;
        AuditHelper.ApplyUpdate(admission, _tenant);
        await _admissions.UpdateAsync(admission, cancellationToken);
        await _admissions.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("HMS Admission {Id} discharged", admission.Id);
        return BaseResponse<AdmissionResponseDto>.Ok(_mapper.Map<AdmissionResponseDto>(admission), "Discharged.");
    }

    private bool IsInCurrentFacility(HmsAdmission admission) =>
        _tenant.FacilityId is long f && admission.FacilityId == f;

    private static string BuildAdmissionNo(long facilityId)
    {
        var suffix = Random.Shared.Next(1000, 9999);
        return $"ADM-{facilityId}-{DateTime.UtcNow:yyyyMMddHHmmss}-{suffix}";
    }
}
