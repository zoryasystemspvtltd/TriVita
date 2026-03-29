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
}
