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

public interface IPhrMedicineBatchService
{
    Task<BaseResponse<MedicineBatchResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<MedicineBatchResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<MedicineBatchResponseDto>> CreateAsync(CreateMedicineBatchDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<MedicineBatchResponseDto>> UpdateAsync(long id, UpdateMedicineBatchDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class PhrMedicineBatchService : PhrCrudServiceBase<PhrMedicineBatch, CreateMedicineBatchDto, UpdateMedicineBatchDto, MedicineBatchResponseDto, PhrMedicineBatchService>, IPhrMedicineBatchService
{
    private const string DuplicateBatchMessage = "Batch number already exists for the selected medicine.";

    public PhrMedicineBatchService(
        IRepository<PhrMedicineBatch> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateMedicineBatchDto>? createValidator,
        IValidator<UpdateMedicineBatchDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<PhrMedicineBatchService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => false;

    public Task<BaseResponse<PagedResponse<MedicineBatchResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);

    public override async Task<BaseResponse<MedicineBatchResponseDto>> CreateAsync(
        CreateMedicineBatchDto dto,
        CancellationToken cancellationToken = default)
    {
        dto.BatchNo = dto.BatchNo?.Trim();

        var exists = await Repository.ListAsync(
            e =>
                e.TenantId == Tenant.TenantId &&
                !e.IsDeleted &&
                e.MedicineId == dto.MedicineId &&
                e.BatchNo == dto.BatchNo,
            cancellationToken);

        if (exists.Count > 0)
            return BaseResponse<MedicineBatchResponseDto>.Fail(DuplicateBatchMessage);

        try
        {
            return await base.CreateAsync(dto, cancellationToken);
        }
        catch
        {
            return BaseResponse<MedicineBatchResponseDto>.Fail(DuplicateBatchMessage);
        }
    }

    public override async Task<BaseResponse<MedicineBatchResponseDto>> UpdateAsync(
        long id,
        UpdateMedicineBatchDto dto,
        CancellationToken cancellationToken = default)
    {
        dto.BatchNo = dto.BatchNo?.Trim();

        var exists = await Repository.ListAsync(
            e =>
                e.TenantId == Tenant.TenantId &&
                !e.IsDeleted &&
                e.Id != id &&
                e.MedicineId == dto.MedicineId &&
                e.BatchNo == dto.BatchNo,
            cancellationToken);

        if (exists.Count > 0)
            return BaseResponse<MedicineBatchResponseDto>.Fail(DuplicateBatchMessage);

        try
        {
            return await base.UpdateAsync(id, dto, cancellationToken);
        }
        catch
        {
            return BaseResponse<MedicineBatchResponseDto>.Fail(DuplicateBatchMessage);
        }
    }
}
