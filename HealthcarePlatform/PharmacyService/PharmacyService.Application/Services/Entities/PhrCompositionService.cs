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
}
