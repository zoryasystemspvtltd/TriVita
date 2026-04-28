using System.Linq;
using AutoMapper;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using PharmacyService.Application.DTOs.Entities;
using PharmacyService.Application.Services;
using PharmacyService.Domain.Entities;
using PharmacyService.Domain.Repositories;
using Healthcare.Common.MultiTenancy;
using Microsoft.Extensions.Logging;

namespace PharmacyService.Application.Services.Entities;

public interface IPhrMedicineFormService
{
    Task<BaseResponse<MedicineFormResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<MedicineFormResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<MedicineFormResponseDto>> CreateAsync(CreateMedicineFormDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<MedicineFormResponseDto>> UpdateAsync(long id, UpdateMedicineFormDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

/// <summary>Medicine dosage forms stored as <see cref="PhrReferenceDataValue"/> under a tenant-specific definition.</summary>
public sealed class PhrMedicineFormService : IPhrMedicineFormService
{
    public const string DosageFormDefinitionCode = "TRIVITA_PHARMACY_MEDICINE_FORM";

    private readonly IRepository<PhrReferenceDataValue> _values;
    private readonly IRepository<PhrReferenceDataDefinition> _definitions;
    private readonly IMapper _mapper;
    private readonly ITenantContext _tenant;
    private readonly ILogger<PhrMedicineFormService> _logger;

    public PhrMedicineFormService(
        IRepository<PhrReferenceDataValue> values,
        IRepository<PhrReferenceDataDefinition> definitions,
        IMapper mapper,
        ITenantContext tenant,
        ILogger<PhrMedicineFormService> logger)
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
            d => d.DefinitionCode == DosageFormDefinitionCode && !d.IsDeleted,
            cancellationToken);
        var existing = defs.FirstOrDefault();
        if (existing != null)
            return existing.Id;

        var created = new PhrReferenceDataDefinition
        {
            DefinitionCode = DosageFormDefinitionCode,
            DefinitionName = "Medicine dosage form",
            Description = "Reference values used as Medicine.FormReferenceValueId",
        };
        AuditHelper.ApplyCreate(created, _tenant);
        await _definitions.AddAsync(created, cancellationToken);
        await _definitions.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Created dosage-form definition for tenant {TenantId} id {Id}", _tenant.TenantId, created.Id);
        return created.Id;
    }

    public async Task<BaseResponse<MedicineFormResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var defId = await GetOrCreateDefinitionIdAsync(cancellationToken);
        var entity = await _values.GetByIdAsync(id, cancellationToken);
        if (entity is null || entity.ReferenceDataDefinitionId != defId || entity.IsDeleted)
            return BaseResponse<MedicineFormResponseDto>.Fail("Form not found.");
        return BaseResponse<MedicineFormResponseDto>.Ok(_mapper.Map<MedicineFormResponseDto>(entity));
    }

    public async Task<BaseResponse<PagedResponse<MedicineFormResponseDto>>> GetPagedAsync(
        PagedQuery query,
        CancellationToken cancellationToken = default)
    {
        var defId = await GetOrCreateDefinitionIdAsync(cancellationToken);
        var (items, total) = await _values.GetPagedByFilterAsync(
            query.Page,
            query.PageSize,
            v => v.ReferenceDataDefinitionId == defId && !v.IsDeleted,
            cancellationToken);

        var dtoItems = _mapper.Map<IReadOnlyList<MedicineFormResponseDto>>(items);
        return BaseResponse<PagedResponse<MedicineFormResponseDto>>.Ok(
            new PagedResponse<MedicineFormResponseDto>
            {
                Items = dtoItems,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = total,
            });
    }

    private async Task<string?> DuplicateMessageAsync(
        long defId,
        string code,
        string name,
        long? excludeId,
        CancellationToken cancellationToken)
    {
        var all = await _values.ListAsync(
            v => v.ReferenceDataDefinitionId == defId && !v.IsDeleted,
            cancellationToken);
        foreach (var v in all)
        {
            if (excludeId is long e && v.Id == e) continue;
            if (string.Equals(v.ValueCode.Trim(), code, StringComparison.OrdinalIgnoreCase))
                return "Form already exists with same name or code.";
            if (string.Equals(v.ValueName.Trim(), name, StringComparison.OrdinalIgnoreCase))
                return "Form already exists with same name or code.";
        }
        return null;
    }

    public async Task<BaseResponse<MedicineFormResponseDto>> CreateAsync(
        CreateMedicineFormDto dto,
        CancellationToken cancellationToken = default)
    {
        var defId = await GetOrCreateDefinitionIdAsync(cancellationToken);
        var code = dto.FormCode.Trim();
        var name = dto.FormName.Trim();
        var dup = await DuplicateMessageAsync(defId, code, name, null, cancellationToken);
        if (dup != null)
            return BaseResponse<MedicineFormResponseDto>.Fail(dup);

        var entity = new PhrReferenceDataValue
        {
            ReferenceDataDefinitionId = defId,
            ValueCode = code,
            ValueName = name,
            ValueText = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
            SortOrder = 0,
        };
        AuditHelper.ApplyCreate(entity, _tenant);
        await _values.AddAsync(entity, cancellationToken);
        await _values.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Medicine form created tenant {TenantId} id {Id}", _tenant.TenantId, entity.Id);
        return BaseResponse<MedicineFormResponseDto>.Ok(_mapper.Map<MedicineFormResponseDto>(entity), "Created.");
    }

    public async Task<BaseResponse<MedicineFormResponseDto>> UpdateAsync(
        long id,
        UpdateMedicineFormDto dto,
        CancellationToken cancellationToken = default)
    {
        var defId = await GetOrCreateDefinitionIdAsync(cancellationToken);
        var entity = await _values.GetByIdAsync(id, cancellationToken);
        if (entity is null || entity.ReferenceDataDefinitionId != defId || entity.IsDeleted)
            return BaseResponse<MedicineFormResponseDto>.Fail("Form not found.");

        var code = dto.FormCode.Trim();
        var name = dto.FormName.Trim();
        var dup = await DuplicateMessageAsync(defId, code, name, id, cancellationToken);
        if (dup != null)
            return BaseResponse<MedicineFormResponseDto>.Fail(dup);

        entity.ValueCode = code;
        entity.ValueName = name;
        entity.ValueText = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim();
        AuditHelper.ApplyUpdate(entity, _tenant);
        await _values.UpdateAsync(entity, cancellationToken);
        await _values.SaveChangesAsync(cancellationToken);
        return BaseResponse<MedicineFormResponseDto>.Ok(_mapper.Map<MedicineFormResponseDto>(entity), "Updated.");
    }

    public async Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var defId = await GetOrCreateDefinitionIdAsync(cancellationToken);
        var entity = await _values.GetByIdAsync(id, cancellationToken);
        if (entity is null || entity.ReferenceDataDefinitionId != defId || entity.IsDeleted)
            return BaseResponse<object?>.Fail("Form not found.");

        entity.IsDeleted = true;
        entity.IsActive = false;
        AuditHelper.ApplyUpdate(entity, _tenant);
        await _values.UpdateAsync(entity, cancellationToken);
        await _values.SaveChangesAsync(cancellationToken);
        return BaseResponse<object?>.Ok(null, "Deleted.");
    }
}
