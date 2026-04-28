using System.Linq;
using AutoMapper;
using FluentValidation;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using PharmacyService.Domain.Entities;
using PharmacyService.Application.DTOs.Entities;
using PharmacyService.Domain.Repositories;
using PharmacyService.Application.Services.Extended;
using Microsoft.Extensions.Logging;

namespace PharmacyService.Application.Services.Entities;

public interface IPhrMedicineService
{
    Task<BaseResponse<MedicineResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<MedicineResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<MedicineResponseDto>> CreateAsync(CreateMedicineDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<MedicineResponseDto>> UpdateAsync(long id, UpdateMedicineDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class PhrMedicineService : PhrCrudServiceBase<PhrMedicine, CreateMedicineDto, UpdateMedicineDto, MedicineResponseDto, PhrMedicineService>, IPhrMedicineService
{
    private const string DuplicateNameMessage = "Medicine already exists with same name or code.";
    private const string DuplicateCodeMessage = "Medicine already exists with same name or code.";

    public PhrMedicineService(
        IRepository<PhrMedicine> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateMedicineDto>? createValidator,
        IValidator<UpdateMedicineDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<PhrMedicineService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => false;

    public Task<BaseResponse<PagedResponse<MedicineResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);

    public override async Task<BaseResponse<MedicineResponseDto>> CreateAsync(
        CreateMedicineDto dto,
        CancellationToken cancellationToken = default)
    {
        dto.MedicineName = dto.MedicineName?.Trim();
        dto.MedicineCode = dto.MedicineCode?.Trim();

        var name = (dto.MedicineName ?? string.Empty).Trim();
        var code = (dto.MedicineCode ?? string.Empty).Trim();

        var dups = await Repository.ListAsync(
            e =>
                e.TenantId == Tenant.TenantId &&
                !e.IsDeleted &&
                (e.MedicineName.ToLower() == name.ToLower() || e.MedicineCode.ToLower() == code.ToLower()),
            cancellationToken);

        if (dups.Count > 0)
            return BaseResponse<MedicineResponseDto>.Fail(DuplicateNameMessage);

        return await base.CreateAsync(dto, cancellationToken);
    }

    public override async Task<BaseResponse<MedicineResponseDto>> UpdateAsync(
        long id,
        UpdateMedicineDto dto,
        CancellationToken cancellationToken = default)
    {
        dto.MedicineName = dto.MedicineName?.Trim();
        dto.MedicineCode = dto.MedicineCode?.Trim();

        var name = (dto.MedicineName ?? string.Empty).Trim();
        var code = (dto.MedicineCode ?? string.Empty).Trim();

        var dups = await Repository.ListAsync(
            e =>
                e.TenantId == Tenant.TenantId &&
                !e.IsDeleted &&
                e.Id != id &&
                (e.MedicineName.ToLower() == name.ToLower() || e.MedicineCode.ToLower() == code.ToLower()),
            cancellationToken);

        if (dups.Count > 0)
            return BaseResponse<MedicineResponseDto>.Fail(DuplicateCodeMessage);

        return await base.UpdateAsync(id, dto, cancellationToken);
    }
}
