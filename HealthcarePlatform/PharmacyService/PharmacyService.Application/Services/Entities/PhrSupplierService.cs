using System.Linq;
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
    private const string DuplicateMessage = "Supplier already exists with same name or code.";
    private const string DuplicatePanMessage = "Supplier already exists with same PAN.";
    private const string DuplicateMsmeMessage = "Supplier already exists with same MSME number.";
    private const string DuplicateTanMessage = "Supplier already exists with same TAN.";
    private const string DuplicateGstMessage = "Supplier already exists with same GST number.";
    private const string DuplicateIecMessage = "Supplier already exists with same Export/Import Code.";
    private const string DuplicateCinMessage = "Supplier already exists with same CIN.";

    private readonly IRepository<PhrSupplier> _suppliers;
    private readonly ITenantContext _tenant;
    private readonly ILogger<PhrSupplierService> _logger;

    public PhrSupplierService(
        IRepository<PhrSupplier> suppliers,
        ITenantContext tenant,
        ILogger<PhrSupplierService> logger)
    {
        _suppliers = suppliers;
        _tenant = tenant;
        _logger = logger;
    }

    private static string? TrimOrNull(string? v)
    {
        var t = v?.Trim();
        return string.IsNullOrWhiteSpace(t) ? null : t;
    }

    private static SupplierResponseDto ToDto(PhrSupplier e) =>
        new()
        {
            Id = e.Id,
            SupplierCode = e.SupplierCode,
            SupplierName = e.SupplierName,
            IsActive = e.IsActive,
            Pan = e.Pan,
            Msme = e.Msme,
            Tan = e.Tan,
            ExportImportCode = e.ExportImportCode,
            GstNo = e.GstNo,
            Cin = e.Cin,
            ContactPerson = e.ContactPerson,
            Phone = e.Phone,
            Email = e.Email,
            Address = e.Address,
            Description = e.Description,
        };

    private async Task<bool> HasDuplicateAsync(
        string code,
        string name,
        long? excludeId,
        CancellationToken cancellationToken)
    {
        var all = await _suppliers.ListAsync(s => !s.IsDeleted, cancellationToken);
        foreach (var s in all)
        {
            if (excludeId is long x && s.Id == x) continue;
            if (string.Equals(s.SupplierCode.Trim(), code, StringComparison.OrdinalIgnoreCase)) return true;
            if (string.Equals(s.SupplierName.Trim(), name, StringComparison.OrdinalIgnoreCase)) return true;
        }
        return false;
    }

    private async Task<bool> HasDuplicatePanAsync(string panUpper, long? excludeId, CancellationToken cancellationToken)
    {
        var all = await _suppliers.ListAsync(s => !s.IsDeleted, cancellationToken);
        foreach (var s in all)
        {
            if (excludeId is long x && s.Id == x) continue;
            if (string.Equals(s.Pan.Trim(), panUpper, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    private async Task<bool> HasDuplicateFieldAsync(
        string? fieldValue,
        Func<PhrSupplier, string?> selector,
        long? excludeId,
        CancellationToken cancellationToken)
    {
        if (fieldValue is null) return false;
        var all = await _suppliers.ListAsync(s => !s.IsDeleted, cancellationToken);
        foreach (var s in all)
        {
            if (excludeId is long x && s.Id == x) continue;
            var existing = selector(s);
            if (existing != null && string.Equals(existing.Trim(), fieldValue, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    public async Task<BaseResponse<SupplierResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _suppliers.GetByIdAsync(id, cancellationToken);
        if (entity is null || entity.IsDeleted)
            return BaseResponse<SupplierResponseDto>.Fail("Supplier not found.");
        return BaseResponse<SupplierResponseDto>.Ok(ToDto(entity));
    }

    public async Task<BaseResponse<PagedResponse<SupplierResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
    {
        var (items, total) = await _suppliers.GetPagedByFilterAsync(
            query.Page,
            query.PageSize,
            s => !s.IsDeleted,
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

        if (await HasDuplicateAsync(dto.SupplierCode, dto.SupplierName, null, cancellationToken))
            return BaseResponse<SupplierResponseDto>.Fail(DuplicateMessage);
        if (await HasDuplicatePanAsync(dto.Pan, null, cancellationToken))
            return BaseResponse<SupplierResponseDto>.Fail(DuplicatePanMessage);
        if (dto.Msme != null && await HasDuplicateFieldAsync(dto.Msme, s => s.Msme, null, cancellationToken))
            return BaseResponse<SupplierResponseDto>.Fail(DuplicateMsmeMessage);
        if (dto.Tan != null && await HasDuplicateFieldAsync(dto.Tan, s => s.Tan, null, cancellationToken))
            return BaseResponse<SupplierResponseDto>.Fail(DuplicateTanMessage);
        if (dto.GstNo != null && await HasDuplicateFieldAsync(dto.GstNo, s => s.GstNo, null, cancellationToken))
            return BaseResponse<SupplierResponseDto>.Fail(DuplicateGstMessage);
        if (dto.ExportImportCode != null && await HasDuplicateFieldAsync(dto.ExportImportCode, s => s.ExportImportCode, null, cancellationToken))
            return BaseResponse<SupplierResponseDto>.Fail(DuplicateIecMessage);
        if (dto.Cin != null && await HasDuplicateFieldAsync(dto.Cin, s => s.Cin, null, cancellationToken))
            return BaseResponse<SupplierResponseDto>.Fail(DuplicateCinMessage);

        var entity = new PhrSupplier
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
        AuditHelper.ApplyCreate(entity, _tenant);
        await _suppliers.AddAsync(entity, cancellationToken);
        await _suppliers.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Supplier created tenant {TenantId} id {Id}", _tenant.TenantId, entity.Id);
        return BaseResponse<SupplierResponseDto>.Ok(ToDto(entity), "Created.");
    }

    public async Task<BaseResponse<SupplierResponseDto>> UpdateAsync(long id, UpdateSupplierDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _suppliers.GetByIdAsync(id, cancellationToken);
        if (entity is null || entity.IsDeleted)
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

        if (await HasDuplicateAsync(dto.SupplierCode, dto.SupplierName, id, cancellationToken))
            return BaseResponse<SupplierResponseDto>.Fail(DuplicateMessage);
        if (await HasDuplicatePanAsync(dto.Pan, id, cancellationToken))
            return BaseResponse<SupplierResponseDto>.Fail(DuplicatePanMessage);
        if (dto.Msme != null && await HasDuplicateFieldAsync(dto.Msme, s => s.Msme, id, cancellationToken))
            return BaseResponse<SupplierResponseDto>.Fail(DuplicateMsmeMessage);
        if (dto.Tan != null && await HasDuplicateFieldAsync(dto.Tan, s => s.Tan, id, cancellationToken))
            return BaseResponse<SupplierResponseDto>.Fail(DuplicateTanMessage);
        if (dto.GstNo != null && await HasDuplicateFieldAsync(dto.GstNo, s => s.GstNo, id, cancellationToken))
            return BaseResponse<SupplierResponseDto>.Fail(DuplicateGstMessage);
        if (dto.ExportImportCode != null && await HasDuplicateFieldAsync(dto.ExportImportCode, s => s.ExportImportCode, id, cancellationToken))
            return BaseResponse<SupplierResponseDto>.Fail(DuplicateIecMessage);
        if (dto.Cin != null && await HasDuplicateFieldAsync(dto.Cin, s => s.Cin, id, cancellationToken))
            return BaseResponse<SupplierResponseDto>.Fail(DuplicateCinMessage);

        entity.SupplierCode = dto.SupplierCode;
        entity.SupplierName = dto.SupplierName;
        entity.Pan = dto.Pan;
        entity.Msme = dto.Msme;
        entity.Tan = dto.Tan;
        entity.ExportImportCode = dto.ExportImportCode;
        entity.GstNo = dto.GstNo;
        entity.Cin = dto.Cin;
        entity.ContactPerson = dto.ContactPerson;
        entity.Phone = dto.Phone;
        entity.Email = dto.Email;
        entity.Address = dto.Address;
        entity.Description = dto.Description;
        entity.IsActive = dto.IsActive;
        AuditHelper.ApplyUpdate(entity, _tenant);

        await _suppliers.UpdateAsync(entity, cancellationToken);
        await _suppliers.SaveChangesAsync(cancellationToken);
        return BaseResponse<SupplierResponseDto>.Ok(ToDto(entity), "Updated.");
    }

    public async Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _suppliers.GetByIdAsync(id, cancellationToken);
        if (entity is null || entity.IsDeleted)
            return BaseResponse<object?>.Fail("Supplier not found.");

        entity.IsDeleted = true;
        entity.IsActive = false;
        AuditHelper.ApplyUpdate(entity, _tenant);
        await _suppliers.UpdateAsync(entity, cancellationToken);
        await _suppliers.SaveChangesAsync(cancellationToken);
        return BaseResponse<object?>.Ok(null, "Deleted.");
    }
}
