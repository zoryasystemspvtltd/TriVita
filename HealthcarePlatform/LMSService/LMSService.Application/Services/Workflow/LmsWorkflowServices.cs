using System.Linq;
using AutoMapper;
using FluentValidation;
using Healthcare.Common.Integration.SharedService;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using LMSService.Application.DTOs.Workflow;
using LMSService.Application.Services;
using LMSService.Application.Services.Extended;
using LMSService.Domain.Entities;
using LMSService.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace LMSService.Application.Services.Workflow;

public interface ILmsWorkflowIntegrationService
{
    Task<BaseResponse<LmsBarcodeResolutionDto>> ResolveBarcodeAsync(string barcodeValue, CancellationToken cancellationToken = default);
}

public sealed class LmsWorkflowIntegrationService : ILmsWorkflowIntegrationService
{
    private readonly IRepository<LmsLabSampleBarcode> _barcodes;
    private readonly IRepository<LmsLabTestBookingItem> _items;
    private readonly IRepository<LmsLabTestBooking> _bookings;
    private readonly IRepository<LmsCatalogTest> _tests;
    private readonly IRepository<LmsCatalogTestParameterMap> _testParamMaps;
    private readonly IRepository<LmsCatalogParameter> _parameters;
    private readonly IRepository<LmsEquipmentTestMaster> _equipmentTests;
    private readonly ITenantContext _tenant;
    private readonly ILogger<LmsWorkflowIntegrationService> _logger;

    public LmsWorkflowIntegrationService(
        IRepository<LmsLabSampleBarcode> barcodes,
        IRepository<LmsLabTestBookingItem> items,
        IRepository<LmsLabTestBooking> bookings,
        IRepository<LmsCatalogTest> tests,
        IRepository<LmsCatalogTestParameterMap> testParamMaps,
        IRepository<LmsCatalogParameter> parameters,
        IRepository<LmsEquipmentTestMaster> equipmentTests,
        ITenantContext tenant,
        ILogger<LmsWorkflowIntegrationService> logger)
    {
        _barcodes = barcodes;
        _items = items;
        _bookings = bookings;
        _tests = tests;
        _testParamMaps = testParamMaps;
        _parameters = parameters;
        _equipmentTests = equipmentTests;
        _tenant = tenant;
        _logger = logger;
    }

    public async Task<BaseResponse<LmsBarcodeResolutionDto>> ResolveBarcodeAsync(
        string barcodeValue,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(barcodeValue))
            return BaseResponse<LmsBarcodeResolutionDto>.Fail("Barcode is required.");

        var trimmed = barcodeValue.Trim();
        var list = await _barcodes.ListAsync(b => b.BarcodeValue == trimmed, cancellationToken);
        var bc = list.FirstOrDefault();
        if (bc is null)
            return BaseResponse<LmsBarcodeResolutionDto>.Fail("Barcode not found.");

        if (_tenant.FacilityId is long fid && bc.FacilityId != fid)
            return BaseResponse<LmsBarcodeResolutionDto>.Fail("Barcode is not in the current facility scope.");

        var item = await _items.GetByIdAsync(bc.TestBookingItemId, cancellationToken);
        if (item is null)
            return BaseResponse<LmsBarcodeResolutionDto>.Fail("Booking item not found.");

        var booking = await _bookings.GetByIdAsync(item.LabTestBookingId, cancellationToken);
        if (booking is null)
            return BaseResponse<LmsBarcodeResolutionDto>.Fail("Booking not found.");

        var test = await _tests.GetByIdAsync(item.CatalogTestId, cancellationToken);
        if (test is null || test.FacilityId != item.FacilityId)
            return BaseResponse<LmsBarcodeResolutionDto>.Fail("Catalog test not found.");

        var maps = await _testParamMaps.ListAsync(
            m => m.CatalogTestId == test.Id && m.FacilityId == test.FacilityId,
            cancellationToken);
        var paramSnapshots = new List<LmsCatalogParameterSnapshotDto>();
        foreach (var m in maps.OrderBy(x => x.DisplayOrder))
        {
            var p = await _parameters.GetByIdAsync(m.CatalogParameterId, cancellationToken);
            if (p is null)
                continue;
            paramSnapshots.Add(new LmsCatalogParameterSnapshotDto
            {
                CatalogParameterId = p.Id,
                ParameterCode = p.ParameterCode,
                ParameterName = p.ParameterName,
                IsNumeric = p.IsNumeric,
                UnitId = p.UnitId
            });
        }

        var assays = await _equipmentTests.ListAsync(
            e => e.CatalogTestId == test.Id && e.FacilityId == test.FacilityId,
            cancellationToken);
        var assayDtos = assays.Select(a => new LmsEquipmentAssayDto
        {
            EquipmentId = a.EquipmentId,
            EquipmentAssayCode = a.EquipmentAssayCode,
            EquipmentAssayName = a.EquipmentAssayName
        }).ToList();

        _logger.LogInformation("LMS barcode resolved {Barcode} item {ItemId}", trimmed, item.Id);

        return BaseResponse<LmsBarcodeResolutionDto>.Ok(new LmsBarcodeResolutionDto
        {
            FacilityId = item.FacilityId ?? 0,
            TestBookingItemId = item.Id,
            LabTestBookingId = booking.Id,
            CatalogTestId = test.Id,
            TestCode = test.TestCode,
            TestName = test.TestName,
            PatientId = booking.PatientId,
            VisitId = booking.VisitId,
            BarcodeValue = bc.BarcodeValue,
            EquipmentAssays = assayDtos,
            Parameters = paramSnapshots
        });
    }
}

public interface ILmsLabTestBookingService
{
    Task<BaseResponse<LmsLabTestBookingResponseDto>> CreateAsync(CreateLmsLabTestBookingDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<LmsLabTestBookingResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<LmsLabTestBookingResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
}

public sealed class LmsLabTestBookingService : ILmsLabTestBookingService
{
    private readonly IRepository<LmsLabTestBooking> _bookings;
    private readonly IRepository<LmsLabTestBookingItem> _items;
    private readonly IRepository<LmsCatalogTest> _tests;
    private readonly ITenantContext _tenant;
    private readonly IFacilityTenantValidator _facilityValidator;
    private readonly IValidator<CreateLmsLabTestBookingDto> _validator;
    private readonly ILogger<LmsLabTestBookingService> _logger;

    public LmsLabTestBookingService(
        IRepository<LmsLabTestBooking> bookings,
        IRepository<LmsLabTestBookingItem> items,
        IRepository<LmsCatalogTest> tests,
        ITenantContext tenant,
        IFacilityTenantValidator facilityValidator,
        IValidator<CreateLmsLabTestBookingDto> validator,
        ILogger<LmsLabTestBookingService> logger)
    {
        _bookings = bookings;
        _items = items;
        _tests = tests;
        _tenant = tenant;
        _facilityValidator = facilityValidator;
        _validator = validator;
        _logger = logger;
    }

    public async Task<BaseResponse<LmsLabTestBookingResponseDto>> CreateAsync(
        CreateLmsLabTestBookingDto dto,
        CancellationToken cancellationToken = default)
    {
        if (_tenant.FacilityId is null)
            return BaseResponse<LmsLabTestBookingResponseDto>.Fail("FacilityId is required.");

        var v = await _validator.ValidateAsync(dto, cancellationToken);
        if (!v.IsValid)
            return BaseResponse<LmsLabTestBookingResponseDto>.Fail("Validation failed.", v.Errors.Select(e => e.ErrorMessage));

        var fid = _tenant.FacilityId.Value;
        var fctx = await _facilityValidator.GetFacilityContextAsync(_tenant.TenantId, fid, cancellationToken);
        if (fctx is null)
            return BaseResponse<LmsLabTestBookingResponseDto>.Fail("Facility is not valid for this tenant.");

        foreach (var line in dto.Items)
        {
            var t = await _tests.GetByIdAsync(line.CatalogTestId, cancellationToken);
            if (t is null || t.FacilityId != fid)
                return BaseResponse<LmsLabTestBookingResponseDto>.Fail($"Catalog test {line.CatalogTestId} is invalid for this facility.");
        }

        var booking = new LmsLabTestBooking
        {
            BookingNo = $"LAB-{fid}-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}",
            PatientId = dto.PatientId,
            VisitId = dto.VisitId,
            SourceReferenceValueId = dto.SourceReferenceValueId,
            BookingNotes = dto.BookingNotes
        };
        AuditHelper.ApplyCreate(booking, _tenant);
        booking.FacilityId = fid;

        await _bookings.AddAsync(booking, cancellationToken);
        await _bookings.SaveChangesAsync(cancellationToken);

        foreach (var line in dto.Items)
        {
            var item = new LmsLabTestBookingItem
            {
                LabTestBookingId = booking.Id,
                CatalogTestId = line.CatalogTestId,
                WorkflowStatusReferenceValueId = line.WorkflowStatusReferenceValueId,
                LineNotes = line.LineNotes
            };
            AuditHelper.ApplyCreate(item, _tenant);
            item.FacilityId = fid;
            await _items.AddAsync(item, cancellationToken);
        }

        await _items.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("LMS LabTestBooking created {Id} {No}", booking.Id, booking.BookingNo);

        return BaseResponse<LmsLabTestBookingResponseDto>.Ok(await BuildResponseAsync(booking.Id, cancellationToken), "Created.");
    }

    public async Task<BaseResponse<LmsLabTestBookingResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var booking = await _bookings.GetByIdAsync(id, cancellationToken);
        if (booking is null)
            return BaseResponse<LmsLabTestBookingResponseDto>.Fail("Booking not found.");
        if (_tenant.FacilityId is long f && booking.FacilityId != f)
            return BaseResponse<LmsLabTestBookingResponseDto>.Fail("Booking not found.");

        return BaseResponse<LmsLabTestBookingResponseDto>.Ok(await BuildResponseAsync(id, cancellationToken));
    }

    public async Task<BaseResponse<PagedResponse<LmsLabTestBookingResponseDto>>> GetPagedAsync(
        PagedQuery query,
        CancellationToken cancellationToken = default)
    {
        if (_tenant.FacilityId is null)
            return BaseResponse<PagedResponse<LmsLabTestBookingResponseDto>>.Fail("FacilityId is required.");

        var fid = _tenant.FacilityId.Value;
        var (rows, total) = await _bookings.GetPagedByFilterAsync(
            query.Page,
            query.PageSize,
            b => b.FacilityId == fid,
            cancellationToken);

        var items = new List<LmsLabTestBookingResponseDto>();
        foreach (var b in rows)
            items.Add(await BuildResponseAsync(b.Id, cancellationToken));

        return BaseResponse<PagedResponse<LmsLabTestBookingResponseDto>>.Ok(new PagedResponse<LmsLabTestBookingResponseDto>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = total
        });
    }

    private async Task<LmsLabTestBookingResponseDto> BuildResponseAsync(long bookingId, CancellationToken cancellationToken)
    {
        var booking = (await _bookings.GetByIdAsync(bookingId, cancellationToken))!;
        var itemRows = await _items.ListAsync(i => i.LabTestBookingId == bookingId, cancellationToken);
        return new LmsLabTestBookingResponseDto
        {
            Id = booking.Id,
            FacilityId = booking.FacilityId ?? 0,
            BookingNo = booking.BookingNo,
            PatientId = booking.PatientId,
            VisitId = booking.VisitId,
            SourceReferenceValueId = booking.SourceReferenceValueId,
            Items = itemRows.Select(i => new LmsLabTestBookingItemResponseDto
            {
                Id = i.Id,
                CatalogTestId = i.CatalogTestId,
                WorkflowStatusReferenceValueId = i.WorkflowStatusReferenceValueId,
                LineNotes = i.LineNotes
            }).ToList()
        };
    }
}

public interface ILmsLabSampleBarcodeService
{
    Task<BaseResponse<LmsLabSampleBarcodeResponseDto>> RegisterAsync(RegisterLmsLabSampleBarcodeDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<LmsLabSampleBarcodeResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsLabSampleBarcodeService : ILmsLabSampleBarcodeService
{
    private readonly IRepository<LmsLabSampleBarcode> _barcodes;
    private readonly IRepository<LmsLabTestBookingItem> _items;
    private readonly ITenantContext _tenant;
    private readonly IFacilityTenantValidator _facilityValidator;
    private readonly IMapper _mapper;
    private readonly IValidator<RegisterLmsLabSampleBarcodeDto> _validator;
    private readonly ILogger<LmsLabSampleBarcodeService> _logger;

    public LmsLabSampleBarcodeService(
        IRepository<LmsLabSampleBarcode> barcodes,
        IRepository<LmsLabTestBookingItem> items,
        ITenantContext tenant,
        IFacilityTenantValidator facilityValidator,
        IMapper mapper,
        IValidator<RegisterLmsLabSampleBarcodeDto> validator,
        ILogger<LmsLabSampleBarcodeService> logger)
    {
        _barcodes = barcodes;
        _items = items;
        _tenant = tenant;
        _facilityValidator = facilityValidator;
        _mapper = mapper;
        _validator = validator;
        _logger = logger;
    }

    public async Task<BaseResponse<LmsLabSampleBarcodeResponseDto>> RegisterAsync(
        RegisterLmsLabSampleBarcodeDto dto,
        CancellationToken cancellationToken = default)
    {
        if (_tenant.FacilityId is null)
            return BaseResponse<LmsLabSampleBarcodeResponseDto>.Fail("FacilityId is required.");

        var v = await _validator.ValidateAsync(dto, cancellationToken);
        if (!v.IsValid)
            return BaseResponse<LmsLabSampleBarcodeResponseDto>.Fail("Validation failed.", v.Errors.Select(e => e.ErrorMessage));

        var fid = _tenant.FacilityId.Value;
        var fctx = await _facilityValidator.GetFacilityContextAsync(_tenant.TenantId, fid, cancellationToken);
        if (fctx is null)
            return BaseResponse<LmsLabSampleBarcodeResponseDto>.Fail("Facility is not valid for this tenant.");

        var existing = await _barcodes.ListAsync(b => b.BarcodeValue == dto.BarcodeValue.Trim(), cancellationToken);
        if (existing.Count > 0)
            return BaseResponse<LmsLabSampleBarcodeResponseDto>.Fail("Barcode already registered.");

        var item = await _items.GetByIdAsync(dto.TestBookingItemId, cancellationToken);
        if (item is null || item.FacilityId != fid)
            return BaseResponse<LmsLabSampleBarcodeResponseDto>.Fail("Booking item not found in this facility.");

        var entity = _mapper.Map<LmsLabSampleBarcode>(dto);
        entity.BarcodeValue = dto.BarcodeValue.Trim();
        AuditHelper.ApplyCreate(entity, _tenant);
        entity.FacilityId = fid;

        await _barcodes.AddAsync(entity, cancellationToken);
        await _barcodes.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("LMS barcode registered {Barcode} id {Id}", entity.BarcodeValue, entity.Id);
        return BaseResponse<LmsLabSampleBarcodeResponseDto>.Ok(_mapper.Map<LmsLabSampleBarcodeResponseDto>(entity), "Registered.");
    }

    public async Task<BaseResponse<LmsLabSampleBarcodeResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _barcodes.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return BaseResponse<LmsLabSampleBarcodeResponseDto>.Fail("Barcode record not found.");
        if (_tenant.FacilityId is long f && entity.FacilityId != f)
            return BaseResponse<LmsLabSampleBarcodeResponseDto>.Fail("Barcode record not found.");

        return BaseResponse<LmsLabSampleBarcodeResponseDto>.Ok(_mapper.Map<LmsLabSampleBarcodeResponseDto>(entity));
    }
}

public interface ILmsEquipmentTypeService
{
    Task<BaseResponse<LmsEquipmentTypeResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<LmsEquipmentTypeResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<LmsEquipmentTypeResponseDto>> CreateAsync(CreateLmsEquipmentTypeDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<LmsEquipmentTypeResponseDto>> UpdateAsync(long id, UpdateLmsEquipmentTypeDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsEquipmentTypeService
    : LmsCrudServiceBase<LmsEquipmentType, CreateLmsEquipmentTypeDto, UpdateLmsEquipmentTypeDto, LmsEquipmentTypeResponseDto, LmsEquipmentTypeService>,
        ILmsEquipmentTypeService
{
    public LmsEquipmentTypeService(
        IRepository<LmsEquipmentType> repository,
        IMapper mapper,
        ITenantContext tenant,
        IValidator<CreateLmsEquipmentTypeDto>? createValidator,
        IValidator<UpdateLmsEquipmentTypeDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<LmsEquipmentTypeService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    protected override bool RequiresFacilityId => false;

    public Task<BaseResponse<PagedResponse<LmsEquipmentTypeResponseDto>>> GetPagedAsync(
        PagedQuery query,
        CancellationToken cancellationToken = default) =>
        GetPagedCoreAsync(query, null, cancellationToken);
}

public interface ILmsEquipmentFacilityMappingService
{
    Task<BaseResponse<LmsEquipmentFacilityMappingResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<LmsEquipmentFacilityMappingResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<LmsEquipmentFacilityMappingResponseDto>> CreateAsync(CreateLmsEquipmentFacilityMappingDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<LmsEquipmentFacilityMappingResponseDto>> UpdateAsync(long id, UpdateLmsEquipmentFacilityMappingDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsEquipmentFacilityMappingService
    : LmsCrudServiceBase<LmsEquipmentFacilityMapping, CreateLmsEquipmentFacilityMappingDto, UpdateLmsEquipmentFacilityMappingDto, LmsEquipmentFacilityMappingResponseDto, LmsEquipmentFacilityMappingService>,
        ILmsEquipmentFacilityMappingService
{
    public LmsEquipmentFacilityMappingService(
        IRepository<LmsEquipmentFacilityMapping> repository,
        IMapper mapper,
        ITenantContext tenant,
        IValidator<CreateLmsEquipmentFacilityMappingDto>? createValidator,
        IValidator<UpdateLmsEquipmentFacilityMappingDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<LmsEquipmentFacilityMappingService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    public Task<BaseResponse<PagedResponse<LmsEquipmentFacilityMappingResponseDto>>> GetPagedAsync(
        PagedQuery query,
        CancellationToken cancellationToken = default) =>
        GetPagedCoreAsync(query, null, cancellationToken);
}

public interface ILmsCatalogTestService
{
    Task<BaseResponse<LmsCatalogTestResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<LmsCatalogTestResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<LmsCatalogTestResponseDto>> CreateAsync(CreateLmsCatalogTestDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<LmsCatalogTestResponseDto>> UpdateAsync(long id, UpdateLmsCatalogTestDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsCatalogTestService
    : LmsCrudServiceBase<LmsCatalogTest, CreateLmsCatalogTestDto, UpdateLmsCatalogTestDto, LmsCatalogTestResponseDto, LmsCatalogTestService>,
        ILmsCatalogTestService
{
    public LmsCatalogTestService(
        IRepository<LmsCatalogTest> repository,
        IMapper mapper,
        ITenantContext tenant,
        IValidator<CreateLmsCatalogTestDto>? createValidator,
        IValidator<UpdateLmsCatalogTestDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<LmsCatalogTestService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    public Task<BaseResponse<PagedResponse<LmsCatalogTestResponseDto>>> GetPagedAsync(
        PagedQuery query,
        CancellationToken cancellationToken = default) =>
        GetPagedCoreAsync(query, null, cancellationToken);
}
