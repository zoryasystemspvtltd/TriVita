using System.Linq;
using AutoMapper;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using Microsoft.Extensions.Logging;
using PharmacyService.Application.DTOs.Entities;
using PharmacyService.Application.Services;
using PharmacyService.Domain.Entities;
using PharmacyService.Domain.Repositories;
using System.Text.RegularExpressions;

namespace PharmacyService.Application.Services.Entities;

public interface IPhrSupplierService
{
    Task<BaseResponse<SupplierResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<SupplierResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<SupplierResponseDto>> CreateAsync(CreateSupplierDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<SupplierResponseDto>> UpdateAsync(long id, UpdateSupplierDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class PhrSupplierService : IPhrSupplierService
{
    public const string SupplierDefinitionCode = "TRIVITA_PHARMACY_SUPPLIER";
    private const string DuplicateMessage = "Supplier already exists with same name or code.";
    private const string DuplicatePanMessage = "Supplier already exists with same PAN.";
    private const string DuplicateMsmeMessage = "Supplier already exists with same MSME number.";
    private const string DuplicateTanMessage = "Supplier already exists with same TAN.";
    private const string DuplicateGstMessage = "Supplier already exists with same GST number.";
    private const string DuplicateIecMessage = "Supplier already exists with same Export/Import Code.";
    private const string DuplicateCinMessage = "Supplier already exists with same CIN.";

    private readonly IRepository<PhrReferenceDataValue> _values;
    private readonly IRepository<PhrReferenceDataDefinition> _definitions;
    private readonly IMapper _mapper;
    private readonly ITenantContext _tenant;
    private readonly ILogger<PhrSupplierService> _logger;

    public PhrSupplierService(
        IRepository<PhrReferenceDataValue> values,
        IRepository<PhrReferenceDataDefinition> definitions,
        IMapper mapper,
        ITenantContext tenant,
        ILogger<PhrSupplierService> logger)
    {
        _values = values;
        _definitions = definitions;
        _mapper = mapper;
        _tenant = tenant;
        _logger = logger;
    }

    private async Task<long> GetOrCreateDefinitionIdAsync(CancellationToken cancellationToken)
    {
        var defs = await _definitions.ListAsync(
            d => d.DefinitionCode == SupplierDefinitionCode && !d.IsDeleted,
            cancellationToken);
        var existing = defs.FirstOrDefault();
        if (existing != null)
            return existing.Id;

        var created = new PhrReferenceDataDefinition
        {
            DefinitionCode = SupplierDefinitionCode,
            DefinitionName = "Supplier",
            Description = "Reference values used for Pharmacy supplier master",
        };
        AuditHelper.ApplyCreate(created, _tenant);
        await _definitions.AddAsync(created, cancellationToken);
        await _definitions.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Created supplier definition tenant {TenantId} id {Id}", _tenant.TenantId, created.Id);
        return created.Id;
    }

    private static string? TrimOrNull(string? v)
    {
        var t = v?.Trim();
        return string.IsNullOrWhiteSpace(t) ? null : t;
    }

    private static string BuildValueText(CreateSupplierDto dto)
    {
        var parts = new[]
        {
            ("PAN", TrimOrNull(dto.Pan)?.ToUpperInvariant()),
            ("MSME", TrimOrNull(dto.Msme)),
            ("TAN", TrimOrNull(dto.Tan)),
            ("IEC", TrimOrNull(dto.ExportImportCode)),
            ("GST", TrimOrNull(dto.GstNo)),
            ("CIN", TrimOrNull(dto.Cin)),
            ("Contact", TrimOrNull(dto.ContactPerson)),
            ("Phone", TrimOrNull(dto.Phone)),
            ("Email", TrimOrNull(dto.Email)),
            ("Address", TrimOrNull(dto.Address)),
            ("Description", TrimOrNull(dto.Description)),
        };
        var s = string.Join("\n", parts.Where(p => p.Item2 != null).Select(p => $"{p.Item1}: {p.Item2}"));
        if (s.Length <= 1000) return s;
        return s[..1000];
    }

    private static SupplierResponseDto ToDto(PhrReferenceDataValue v)
    {
        var dto = new SupplierResponseDto
        {
            Id = v.Id,
            SupplierCode = v.ValueCode,
            SupplierName = v.ValueName,
            IsActive = v.IsActive,
            Pan = string.Empty,
        };

        var text = v.ValueText ?? string.Empty;
        foreach (var line in text.Split('\n'))
        {
            var idx = line.IndexOf(':');
            if (idx <= 0) continue;
            var k = line[..idx].Trim();
            var val = line[(idx + 1)..].Trim();
            if (val.Length == 0) continue;
            if (k.Equals("PAN", StringComparison.OrdinalIgnoreCase)) dto.Pan = val;
            else if (k.Equals("MSME", StringComparison.OrdinalIgnoreCase)) dto.Msme = val;
            else if (k.Equals("TAN", StringComparison.OrdinalIgnoreCase)) dto.Tan = val;
            else if (k.Equals("IEC", StringComparison.OrdinalIgnoreCase)) dto.ExportImportCode = val;
            else if (k.Equals("GST", StringComparison.OrdinalIgnoreCase)) dto.GstNo = val;
            else if (k.Equals("CIN", StringComparison.OrdinalIgnoreCase)) dto.Cin = val;
            else if (k.Equals("Contact", StringComparison.OrdinalIgnoreCase)) dto.ContactPerson = val;
            else if (k.Equals("Phone", StringComparison.OrdinalIgnoreCase)) dto.Phone = val;
            else if (k.Equals("Email", StringComparison.OrdinalIgnoreCase)) dto.Email = val;
            else if (k.Equals("Address", StringComparison.OrdinalIgnoreCase)) dto.Address = val;
            else if (k.Equals("Description", StringComparison.OrdinalIgnoreCase)) dto.Description = val;
        }

        return dto;
    }

    private async Task<bool> HasDuplicateAsync(
        long defId,
        string code,
        string name,
        long? excludeId,
        CancellationToken cancellationToken)
    {
        var all = await _values.ListAsync(v => v.ReferenceDataDefinitionId == defId && !v.IsDeleted, cancellationToken);
        foreach (var v in all)
        {
            if (excludeId is long e && v.Id == e) continue;
            if (string.Equals(v.ValueCode.Trim(), code, StringComparison.OrdinalIgnoreCase)) return true;
            if (string.Equals(v.ValueName.Trim(), name, StringComparison.OrdinalIgnoreCase)) return true;
        }
        return false;
    }

    private static string? ExtractValueFromText(string? text, string key)
    {
        if (string.IsNullOrWhiteSpace(text)) return null;
        foreach (var line in text.Split('\n'))
        {
            var idx = line.IndexOf(':');
            if (idx <= 0) continue;
            var k = line[..idx].Trim();
            if (!k.Equals(key, StringComparison.OrdinalIgnoreCase)) continue;
            var val = line[(idx + 1)..].Trim();
            return val.Length == 0 ? null : val;
        }
        return null;
    }

    private async Task<bool> HasDuplicatePanAsync(
        long defId,
        string panUpper,
        long? excludeId,
        CancellationToken cancellationToken)
    {
        var all = await _values.ListAsync(v => v.ReferenceDataDefinitionId == defId && !v.IsDeleted, cancellationToken);
        foreach (var v in all)
        {
            if (excludeId is long e && v.Id == e) continue;
            var p = ExtractValueFromText(v.ValueText, "PAN");
            if (p != null && string.Equals(p.Trim(), panUpper, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    private async Task<bool> HasDuplicateTextFieldAsync(
        long defId,
        string key,
        string value,
        long? excludeId,
        CancellationToken cancellationToken)
    {
        var all = await _values.ListAsync(v => v.ReferenceDataDefinitionId == defId && !v.IsDeleted, cancellationToken);
        foreach (var v in all)
        {
            if (excludeId is long e && v.Id == e) continue;
            var existing = ExtractValueFromText(v.ValueText, key);
            if (existing != null && string.Equals(existing.Trim(), value, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    public async Task<BaseResponse<SupplierResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var defId = await GetOrCreateDefinitionIdAsync(cancellationToken);
        var entity = await _values.GetByIdAsync(id, cancellationToken);
        if (entity is null || entity.ReferenceDataDefinitionId != defId || entity.IsDeleted)
            return BaseResponse<SupplierResponseDto>.Fail("Supplier not found.");
        return BaseResponse<SupplierResponseDto>.Ok(ToDto(entity));
    }

    public async Task<BaseResponse<PagedResponse<SupplierResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
    {
        var defId = await GetOrCreateDefinitionIdAsync(cancellationToken);
        var (items, total) = await _values.GetPagedByFilterAsync(
            query.Page,
            query.PageSize,
            v => v.ReferenceDataDefinitionId == defId && !v.IsDeleted,
            cancellationToken);

        var dtoItems = items.Select(ToDto).ToList();
        return BaseResponse<PagedResponse<SupplierResponseDto>>.Ok(
            new PagedResponse<SupplierResponseDto>
            {
                Items = dtoItems,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = total,
            });
    }

    public async Task<BaseResponse<SupplierResponseDto>> CreateAsync(CreateSupplierDto dto, CancellationToken cancellationToken = default)
    {
        var defId = await GetOrCreateDefinitionIdAsync(cancellationToken);
        dto.SupplierCode = dto.SupplierCode.Trim();
        dto.SupplierName = dto.SupplierName.Trim();
        dto.Pan = dto.Pan.Trim().ToUpperInvariant();
        dto.Msme = TrimOrNull(dto.Msme);
        dto.Tan = TrimOrNull(dto.Tan);
        dto.ExportImportCode = TrimOrNull(dto.ExportImportCode);
        dto.GstNo = TrimOrNull(dto.GstNo);
        dto.Cin = TrimOrNull(dto.Cin);
        dto.ContactPerson = TrimOrNull(dto.ContactPerson);
        dto.Phone = TrimOrNull(dto.Phone);
        dto.Email = TrimOrNull(dto.Email);
        dto.Address = TrimOrNull(dto.Address);
        dto.Description = TrimOrNull(dto.Description);

        if (dto.Pan.Length != 10)
            return BaseResponse<SupplierResponseDto>.Fail("PAN must be 10 characters.");
        if (!Regex.IsMatch(dto.Pan, "^[A-Z]{5}[0-9]{4}[A-Z]{1}$"))
            return BaseResponse<SupplierResponseDto>.Fail("Invalid PAN format.");
        if (dto.GstNo != null && dto.GstNo.Trim().Length != 15)
            return BaseResponse<SupplierResponseDto>.Fail("GST No. must be 15 characters.");

        if (await HasDuplicateAsync(defId, dto.SupplierCode, dto.SupplierName, null, cancellationToken))
            return BaseResponse<SupplierResponseDto>.Fail(DuplicateMessage);
        if (await HasDuplicatePanAsync(defId, dto.Pan, null, cancellationToken))
            return BaseResponse<SupplierResponseDto>.Fail(DuplicatePanMessage);
        if (dto.Msme != null && await HasDuplicateTextFieldAsync(defId, "MSME", dto.Msme, null, cancellationToken))
            return BaseResponse<SupplierResponseDto>.Fail(DuplicateMsmeMessage);
        if (dto.Tan != null && await HasDuplicateTextFieldAsync(defId, "TAN", dto.Tan, null, cancellationToken))
            return BaseResponse<SupplierResponseDto>.Fail(DuplicateTanMessage);
        if (dto.GstNo != null && await HasDuplicateTextFieldAsync(defId, "GST", dto.GstNo, null, cancellationToken))
            return BaseResponse<SupplierResponseDto>.Fail(DuplicateGstMessage);
        if (dto.ExportImportCode != null && await HasDuplicateTextFieldAsync(defId, "IEC", dto.ExportImportCode, null, cancellationToken))
            return BaseResponse<SupplierResponseDto>.Fail(DuplicateIecMessage);
        if (dto.Cin != null && await HasDuplicateTextFieldAsync(defId, "CIN", dto.Cin, null, cancellationToken))
            return BaseResponse<SupplierResponseDto>.Fail(DuplicateCinMessage);

        var entity = new PhrReferenceDataValue
        {
            ReferenceDataDefinitionId = defId,
            ValueCode = dto.SupplierCode,
            ValueName = dto.SupplierName,
            ValueText = BuildValueText(dto),
            SortOrder = 0,
            IsActive = dto.IsActive,
        };
        AuditHelper.ApplyCreate(entity, _tenant);
        await _values.AddAsync(entity, cancellationToken);
        await _values.SaveChangesAsync(cancellationToken);
        return BaseResponse<SupplierResponseDto>.Ok(ToDto(entity), "Created.");
    }

    public async Task<BaseResponse<SupplierResponseDto>> UpdateAsync(long id, UpdateSupplierDto dto, CancellationToken cancellationToken = default)
    {
        var defId = await GetOrCreateDefinitionIdAsync(cancellationToken);
        var entity = await _values.GetByIdAsync(id, cancellationToken);
        if (entity is null || entity.ReferenceDataDefinitionId != defId || entity.IsDeleted)
            return BaseResponse<SupplierResponseDto>.Fail("Supplier not found.");

        dto.SupplierCode = dto.SupplierCode.Trim();
        dto.SupplierName = dto.SupplierName.Trim();
        dto.Pan = dto.Pan.Trim().ToUpperInvariant();
        dto.Msme = TrimOrNull(dto.Msme);
        dto.Tan = TrimOrNull(dto.Tan);
        dto.ExportImportCode = TrimOrNull(dto.ExportImportCode);
        dto.GstNo = TrimOrNull(dto.GstNo);
        dto.Cin = TrimOrNull(dto.Cin);
        dto.ContactPerson = TrimOrNull(dto.ContactPerson);
        dto.Phone = TrimOrNull(dto.Phone);
        dto.Email = TrimOrNull(dto.Email);
        dto.Address = TrimOrNull(dto.Address);
        dto.Description = TrimOrNull(dto.Description);

        if (dto.Pan.Length != 10)
            return BaseResponse<SupplierResponseDto>.Fail("PAN must be 10 characters.");
        if (!Regex.IsMatch(dto.Pan, "^[A-Z]{5}[0-9]{4}[A-Z]{1}$"))
            return BaseResponse<SupplierResponseDto>.Fail("Invalid PAN format.");
        if (dto.GstNo != null && dto.GstNo.Trim().Length != 15)
            return BaseResponse<SupplierResponseDto>.Fail("GST No. must be 15 characters.");

        if (await HasDuplicateAsync(defId, dto.SupplierCode, dto.SupplierName, id, cancellationToken))
            return BaseResponse<SupplierResponseDto>.Fail(DuplicateMessage);
        if (await HasDuplicatePanAsync(defId, dto.Pan, id, cancellationToken))
            return BaseResponse<SupplierResponseDto>.Fail(DuplicatePanMessage);
        if (dto.Msme != null && await HasDuplicateTextFieldAsync(defId, "MSME", dto.Msme, id, cancellationToken))
            return BaseResponse<SupplierResponseDto>.Fail(DuplicateMsmeMessage);
        if (dto.Tan != null && await HasDuplicateTextFieldAsync(defId, "TAN", dto.Tan, id, cancellationToken))
            return BaseResponse<SupplierResponseDto>.Fail(DuplicateTanMessage);
        if (dto.GstNo != null && await HasDuplicateTextFieldAsync(defId, "GST", dto.GstNo, id, cancellationToken))
            return BaseResponse<SupplierResponseDto>.Fail(DuplicateGstMessage);
        if (dto.ExportImportCode != null && await HasDuplicateTextFieldAsync(defId, "IEC", dto.ExportImportCode, id, cancellationToken))
            return BaseResponse<SupplierResponseDto>.Fail(DuplicateIecMessage);
        if (dto.Cin != null && await HasDuplicateTextFieldAsync(defId, "CIN", dto.Cin, id, cancellationToken))
            return BaseResponse<SupplierResponseDto>.Fail(DuplicateCinMessage);

        var cdto = new CreateSupplierDto
        {
            SupplierCode = dto.SupplierCode,
            SupplierName = dto.SupplierName,
            Pan = dto.Pan,
            Msme = dto.Msme,
            Tan = dto.Tan,
            ExportImportCode = dto.ExportImportCode,
            GstNo = dto.GstNo,
            Cin = dto.Cin,
            ContactPerson = dto.ContactPerson,
            Phone = dto.Phone,
            Email = dto.Email,
            Address = dto.Address,
            Description = dto.Description,
            IsActive = dto.IsActive,
        };

        entity.ValueCode = dto.SupplierCode;
        entity.ValueName = dto.SupplierName;
        entity.ValueText = BuildValueText(cdto);
        entity.IsActive = dto.IsActive;
        AuditHelper.ApplyUpdate(entity, _tenant);

        await _values.UpdateAsync(entity, cancellationToken);
        await _values.SaveChangesAsync(cancellationToken);
        return BaseResponse<SupplierResponseDto>.Ok(ToDto(entity), "Updated.");
    }

    public async Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var defId = await GetOrCreateDefinitionIdAsync(cancellationToken);
        var entity = await _values.GetByIdAsync(id, cancellationToken);
        if (entity is null || entity.ReferenceDataDefinitionId != defId || entity.IsDeleted)
            return BaseResponse<object?>.Fail("Supplier not found.");

        entity.IsDeleted = true;
        entity.IsActive = false;
        AuditHelper.ApplyUpdate(entity, _tenant);
        await _values.UpdateAsync(entity, cancellationToken);
        await _values.SaveChangesAsync(cancellationToken);
        return BaseResponse<object?>.Ok(null, "Deleted.");
    }
}

