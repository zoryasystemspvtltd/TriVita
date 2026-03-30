using System.Linq;
using FluentValidation;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Responses;
using LISService.Application.Abstractions;
using LISService.Application.DTOs.Analyzer;
using LISService.Application.Services;
using LISService.Domain.Entities;
using LISService.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace LISService.Application.Services.Analyzer;

public interface ILisAnalyzerIntegrationService
{
    Task<BaseResponse<AnalyzerQueryTestResponseDto>> QueryTestAsync(string barcodeValue, CancellationToken cancellationToken = default);

    Task<BaseResponse<AnalyzerResultIngestResponseDto>> IngestResultAsync(
        AnalyzerResultIngestDto dto,
        CancellationToken cancellationToken = default);
}

public sealed class LisAnalyzerIntegrationService : ILisAnalyzerIntegrationService
{
    private readonly ILmsWorkflowApiClient _lms;
    private readonly IRepository<LisAnalyzerResultHeader> _headers;
    private readonly IRepository<LisAnalyzerResultLine> _lines;
    private readonly ITenantContext _tenant;
    private readonly IValidator<AnalyzerResultIngestDto> _validator;
    private readonly ILisNotificationHelper _notifications;
    private readonly ILogger<LisAnalyzerIntegrationService> _logger;

    public LisAnalyzerIntegrationService(
        ILmsWorkflowApiClient lms,
        IRepository<LisAnalyzerResultHeader> headers,
        IRepository<LisAnalyzerResultLine> lines,
        ITenantContext tenant,
        IValidator<AnalyzerResultIngestDto> validator,
        ILisNotificationHelper notifications,
        ILogger<LisAnalyzerIntegrationService> logger)
    {
        _lms = lms;
        _headers = headers;
        _lines = lines;
        _tenant = tenant;
        _validator = validator;
        _notifications = notifications;
        _logger = logger;
    }

    public async Task<BaseResponse<AnalyzerQueryTestResponseDto>> QueryTestAsync(
        string barcodeValue,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(barcodeValue))
            return BaseResponse<AnalyzerQueryTestResponseDto>.Fail("Barcode is required.");

        if (_tenant.FacilityId is null)
            return BaseResponse<AnalyzerQueryTestResponseDto>.Fail("FacilityId is required.");

        var res = await _lms.ResolveBarcodeAsync(barcodeValue.Trim(), cancellationToken);
        if (res is null || !res.Success || res.Data is null)
            return BaseResponse<AnalyzerQueryTestResponseDto>.Fail(res?.Message ?? "LMS resolution failed.");

        var data = res.Data;
        return BaseResponse<AnalyzerQueryTestResponseDto>.Ok(new AnalyzerQueryTestResponseDto
        {
            BarcodeValue = data.BarcodeValue,
            LmsCatalogTestId = data.CatalogTestId,
            TestCode = data.TestCode,
            EquipmentTestCodes = data.EquipmentAssays.Select(a => a.EquipmentAssayCode).ToList(),
            EquipmentAssays = data.EquipmentAssays.Select(a => new AnalyzerEquipmentAssayDto
            {
                EquipmentId = a.EquipmentId,
                EquipmentAssayCode = a.EquipmentAssayCode,
                EquipmentAssayName = a.EquipmentAssayName
            }).ToList()
        });
    }

    public async Task<BaseResponse<AnalyzerResultIngestResponseDto>> IngestResultAsync(
        AnalyzerResultIngestDto dto,
        CancellationToken cancellationToken = default)
    {
        if (_tenant.FacilityId is null)
            return BaseResponse<AnalyzerResultIngestResponseDto>.Fail("FacilityId is required.");

        var v = await _validator.ValidateAsync(dto, cancellationToken);
        if (!v.IsValid)
            return BaseResponse<AnalyzerResultIngestResponseDto>.Fail("Validation failed.", v.Errors.Select(e => e.ErrorMessage));

        var fid = _tenant.FacilityId.Value;

        var res = await _lms.ResolveBarcodeAsync(dto.Barcode.Trim(), cancellationToken);
        if (res is null || !res.Success || res.Data is null)
            return BaseResponse<AnalyzerResultIngestResponseDto>.Fail(res?.Message ?? "LMS resolution failed.");

        var data = res.Data;
        if (data.FacilityId != fid)
            return BaseResponse<AnalyzerResultIngestResponseDto>.Fail("Barcode belongs to a different facility.");

        var assay = data.EquipmentAssays.FirstOrDefault(a =>
            string.Equals(a.EquipmentAssayCode, dto.EquipmentTestCode, StringComparison.OrdinalIgnoreCase));
        if (assay is null)
            return BaseResponse<AnalyzerResultIngestResponseDto>.Fail("Equipment test code is not mapped for this catalog test.");

        if (dto.EquipmentId is long eqId && eqId != assay.EquipmentId)
            return BaseResponse<AnalyzerResultIngestResponseDto>.Fail("EquipmentId does not match assay mapping.");

        var header = new LisAnalyzerResultHeader
        {
            BarcodeValue = data.BarcodeValue,
            LmsTestBookingItemId = data.TestBookingItemId,
            LmsCatalogTestId = data.CatalogTestId,
            EquipmentId = assay.EquipmentId,
            EquipmentAssayCode = dto.EquipmentTestCode,
            ReceivedOn = DateTime.UtcNow,
            TechnicallyVerified = dto.TechnicallyVerified,
            TechnicallyVerifiedOn = dto.TechnicallyVerified ? DateTime.UtcNow : null,
            ReadyForDispatch = dto.ReadyForDispatch,
            ResultStatusReferenceValueId = dto.ResultHeaderStatusReferenceValueId
        };
        AuditHelper.ApplyCreate(header, _tenant);
        header.FacilityId = fid;

        await _headers.AddAsync(header, cancellationToken);
        await _headers.SaveChangesAsync(cancellationToken);

        foreach (var val in dto.Values)
        {
            long? paramId = val.LmsCatalogParameterId;
            if (paramId is null && !string.IsNullOrWhiteSpace(val.EquipmentResultCode))
            {
                var match = data.Parameters.FirstOrDefault(p =>
                    string.Equals(p.ParameterCode, val.EquipmentResultCode, StringComparison.OrdinalIgnoreCase));
                paramId = match?.CatalogParameterId;
            }

            var line = new LisAnalyzerResultLine
            {
                AnalyzerResultHeaderId = header.Id,
                LmsCatalogParameterId = paramId,
                EquipmentResultCode = val.EquipmentResultCode,
                ResultNumeric = val.Numeric,
                ResultText = val.Text,
                ResultUnitId = val.UnitId,
                LineStatusReferenceValueId = dto.ResultLineStatusReferenceValueId
            };
            AuditHelper.ApplyCreate(line, _tenant);
            line.FacilityId = fid;
            await _lines.AddAsync(line, cancellationToken);
        }

        await _lines.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("LIS analyzer result header {Id} for barcode {Barcode}", header.Id, dto.Barcode);

        if (dto.ReadyForDispatch && !string.IsNullOrWhiteSpace(dto.PatientEmail))
        {
            await _notifications.NotifyAnalyzerResultReadyAsync(
                header.LmsTestBookingItemId,
                data.PatientId,
                dto.PatientEmail,
                cancellationToken);
        }

        return BaseResponse<AnalyzerResultIngestResponseDto>.Ok(
            new AnalyzerResultIngestResponseDto { AnalyzerResultHeaderId = header.Id },
            "Ingested.");
    }
}
