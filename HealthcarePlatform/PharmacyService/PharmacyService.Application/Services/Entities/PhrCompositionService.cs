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

public interface IPhrCompositionService
{
    Task<BaseResponse<CompositionResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<CompositionResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<CompositionResponseDto>> CreateAsync(CreateCompositionDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<CompositionResponseDto>> UpdateAsync(long id, UpdateCompositionDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class PhrCompositionService : PhrCrudServiceBase<PhrComposition, CreateCompositionDto, UpdateCompositionDto, CompositionResponseDto, PhrCompositionService>, IPhrCompositionService
{
    private const string DuplicateMessage = "Composition already exists with same name, strength, and unit.";

    public PhrCompositionService(
        IRepository<PhrComposition> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateCompositionDto>? createValidator,
        IValidator<UpdateCompositionDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<PhrCompositionService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => false;

    public Task<BaseResponse<PagedResponse<CompositionResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);

    public override async Task<BaseResponse<CompositionResponseDto>> CreateAsync(
        CreateCompositionDto dto,
        CancellationToken cancellationToken = default)
    {
        dto.CompositionName = dto.CompositionName?.Trim();
        dto.CompositionCode = dto.CompositionCode?.Trim();

        var name = (dto.CompositionName ?? string.Empty).Trim();
        var code = dto.CompositionCode?.Trim();

        var dups = await Repository.ListAsync(
            e =>
                e.TenantId == Tenant.TenantId &&
                !e.IsDeleted &&
                (e.CompositionName.ToLower() == name.ToLower() ||
                 (code != null && e.CompositionCode != null && e.CompositionCode.ToLower() == code.ToLower())),
            cancellationToken);

        if (dups.Count > 0)
            return BaseResponse<CompositionResponseDto>.Fail(DuplicateMessage);

        return await base.CreateAsync(dto, cancellationToken);
    }

    public override async Task<BaseResponse<CompositionResponseDto>> UpdateAsync(
        long id,
        UpdateCompositionDto dto,
        CancellationToken cancellationToken = default)
    {
        dto.CompositionName = dto.CompositionName?.Trim();
        dto.CompositionCode = dto.CompositionCode?.Trim();

        var name = (dto.CompositionName ?? string.Empty).Trim();
        var code = dto.CompositionCode?.Trim();

        var dups = await Repository.ListAsync(
            e =>
                e.TenantId == Tenant.TenantId &&
                !e.IsDeleted &&
                e.Id != id &&
                (e.CompositionName.ToLower() == name.ToLower() ||
                 (code != null && e.CompositionCode != null && e.CompositionCode.ToLower() == code.ToLower())),
            cancellationToken);

        if (dups.Count > 0)
            return BaseResponse<CompositionResponseDto>.Fail(DuplicateMessage);

        return await base.UpdateAsync(id, dto, cancellationToken);
    }
}
