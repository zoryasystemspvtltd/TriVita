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

/// <summary>Medicine dosage forms stored in dedicated <see cref="PhrForm"/> master table.</summary>
public sealed class PhrMedicineFormService : IPhrMedicineFormService
{
    private readonly IRepository<PhrForm> _forms;
    private readonly IMapper _mapper;
    private readonly ITenantContext _tenant;
    private readonly ILogger<PhrMedicineFormService> _logger;

    public PhrMedicineFormService(
        IRepository<PhrForm> forms,
        IMapper mapper,
        ITenantContext tenant,
        ILogger<PhrMedicineFormService> logger)
    {
        _forms = forms;
        _mapper = mapper;
        _tenant = tenant;
        _logger = logger;
    }

    public async Task<BaseResponse<MedicineFormResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _forms.GetByIdAsync(id, cancellationToken);
        if (entity is null || entity.IsDeleted)
            return BaseResponse<MedicineFormResponseDto>.Fail("Form not found.");
        return BaseResponse<MedicineFormResponseDto>.Ok(_mapper.Map<MedicineFormResponseDto>(entity));
    }

    public async Task<BaseResponse<PagedResponse<MedicineFormResponseDto>>> GetPagedAsync(
        PagedQuery query,
        CancellationToken cancellationToken = default)
    {
        var (items, total) = await _forms.GetPagedByFilterAsync(
            query.Page,
            query.PageSize,
            f => !f.IsDeleted,
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
        string code,
        string name,
        long? excludeId,
        CancellationToken cancellationToken)
    {
        var all = await _forms.ListAsync(
            f => !f.IsDeleted,
            cancellationToken);
        foreach (var f in all)
        {
            if (excludeId is long e && f.Id == e) continue;
            if (string.Equals(f.FormCode.Trim(), code, StringComparison.OrdinalIgnoreCase))
                return "Form already exists with same name or code.";
            if (string.Equals(f.FormName.Trim(), name, StringComparison.OrdinalIgnoreCase))
                return "Form already exists with same name or code.";
        }
        return null;
    }

    public async Task<BaseResponse<MedicineFormResponseDto>> CreateAsync(
        CreateMedicineFormDto dto,
        CancellationToken cancellationToken = default)
    {
        var code = dto.FormCode.Trim();
        var name = dto.FormName.Trim();
        var dup = await DuplicateMessageAsync(code, name, null, cancellationToken);
        if (dup != null)
            return BaseResponse<MedicineFormResponseDto>.Fail(dup);

        var entity = new PhrForm
        {
            FormCode = code,
            FormName = name,
            Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
        };
        AuditHelper.ApplyCreate(entity, _tenant);
        await _forms.AddAsync(entity, cancellationToken);
        await _forms.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Medicine form created tenant {TenantId} id {Id}", _tenant.TenantId, entity.Id);
        return BaseResponse<MedicineFormResponseDto>.Ok(_mapper.Map<MedicineFormResponseDto>(entity), "Created.");
    }

    public async Task<BaseResponse<MedicineFormResponseDto>> UpdateAsync(
        long id,
        UpdateMedicineFormDto dto,
        CancellationToken cancellationToken = default)
    {
        var entity = await _forms.GetByIdAsync(id, cancellationToken);
        if (entity is null || entity.IsDeleted)
            return BaseResponse<MedicineFormResponseDto>.Fail("Form not found.");

        var code = dto.FormCode.Trim();
        var name = dto.FormName.Trim();
        var dup = await DuplicateMessageAsync(code, name, id, cancellationToken);
        if (dup != null)
            return BaseResponse<MedicineFormResponseDto>.Fail(dup);

        entity.FormCode = code;
        entity.FormName = name;
        entity.Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim();
        AuditHelper.ApplyUpdate(entity, _tenant);
        await _forms.UpdateAsync(entity, cancellationToken);
        await _forms.SaveChangesAsync(cancellationToken);
        return BaseResponse<MedicineFormResponseDto>.Ok(_mapper.Map<MedicineFormResponseDto>(entity), "Updated.");
    }

    public async Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _forms.GetByIdAsync(id, cancellationToken);
        if (entity is null || entity.IsDeleted)
            return BaseResponse<object?>.Fail("Form not found.");

        entity.IsDeleted = true;
        entity.IsActive = false;
        AuditHelper.ApplyUpdate(entity, _tenant);
        await _forms.UpdateAsync(entity, cancellationToken);
        await _forms.SaveChangesAsync(cancellationToken);
        return BaseResponse<object?>.Ok(null, "Deleted.");
    }
}
