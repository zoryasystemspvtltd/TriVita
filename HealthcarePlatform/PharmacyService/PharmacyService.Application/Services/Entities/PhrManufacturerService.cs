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

public interface IPhrManufacturerService
{
    Task<BaseResponse<ManufacturerResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<ManufacturerResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<ManufacturerResponseDto>> CreateAsync(CreateManufacturerDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<ManufacturerResponseDto>> UpdateAsync(long id, UpdateManufacturerDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class PhrManufacturerService : PhrCrudServiceBase<PhrManufacturer, CreateManufacturerDto, UpdateManufacturerDto, ManufacturerResponseDto, PhrManufacturerService>, IPhrManufacturerService
{
    private const string DuplicateNameMessage = "Manufacturer name already exists.";
    private const string DuplicateCodeMessage = "Manufacturer code already exists.";

    public PhrManufacturerService(
        IRepository<PhrManufacturer> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateManufacturerDto>? createValidator,
        IValidator<UpdateManufacturerDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<PhrManufacturerService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => false;

    public Task<BaseResponse<PagedResponse<ManufacturerResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);

    public override async Task<BaseResponse<ManufacturerResponseDto>> CreateAsync(
        CreateManufacturerDto dto,
        CancellationToken cancellationToken = default)
    {
        dto.ManufacturerName = dto.ManufacturerName?.Trim();
        dto.ManufacturerCode = dto.ManufacturerCode?.Trim();

        var name = (dto.ManufacturerName ?? string.Empty).Trim();
        var code = dto.ManufacturerCode?.Trim();

        var dups = await Repository.ListAsync(
            e =>
                e.TenantId == Tenant.TenantId &&
                !e.IsDeleted &&
                (e.ManufacturerName.ToLower() == name.ToLower() ||
                 (code != null && e.ManufacturerCode != null && e.ManufacturerCode.ToLower() == code.ToLower())),
            cancellationToken);

        if (dups.Any(e => e.ManufacturerName.Equals(name, StringComparison.OrdinalIgnoreCase)))
            return BaseResponse<ManufacturerResponseDto>.Fail(DuplicateNameMessage);
        if (code != null && dups.Any(e => (e.ManufacturerCode ?? string.Empty).Equals(code, StringComparison.OrdinalIgnoreCase)))
            return BaseResponse<ManufacturerResponseDto>.Fail(DuplicateCodeMessage);

        return await base.CreateAsync(dto, cancellationToken);
    }

    public override async Task<BaseResponse<ManufacturerResponseDto>> UpdateAsync(
        long id,
        UpdateManufacturerDto dto,
        CancellationToken cancellationToken = default)
    {
        dto.ManufacturerName = dto.ManufacturerName?.Trim();
        dto.ManufacturerCode = dto.ManufacturerCode?.Trim();

        var name = (dto.ManufacturerName ?? string.Empty).Trim();
        var code = dto.ManufacturerCode?.Trim();

        var dups = await Repository.ListAsync(
            e =>
                e.TenantId == Tenant.TenantId &&
                !e.IsDeleted &&
                e.Id != id &&
                (e.ManufacturerName.ToLower() == name.ToLower() ||
                 (code != null && e.ManufacturerCode != null && e.ManufacturerCode.ToLower() == code.ToLower())),
            cancellationToken);

        if (dups.Any(e => e.ManufacturerName.Equals(name, StringComparison.OrdinalIgnoreCase)))
            return BaseResponse<ManufacturerResponseDto>.Fail(DuplicateNameMessage);
        if (code != null && dups.Any(e => (e.ManufacturerCode ?? string.Empty).Equals(code, StringComparison.OrdinalIgnoreCase)))
            return BaseResponse<ManufacturerResponseDto>.Fail(DuplicateCodeMessage);

        return await base.UpdateAsync(id, dto, cancellationToken);
    }
}
