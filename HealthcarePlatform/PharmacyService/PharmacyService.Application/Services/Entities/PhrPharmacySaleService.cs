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

public interface IPhrPharmacySaleService
{
    Task<BaseResponse<PharmacySaleResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<PharmacySaleResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<PharmacySaleResponseDto>> CreateAsync(CreatePharmacySaleDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<PharmacySaleResponseDto>> UpdateAsync(long id, UpdatePharmacySaleDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class PhrPharmacySaleService : PhrCrudServiceBase<PhrPharmacySale, CreatePharmacySaleDto, UpdatePharmacySaleDto, PharmacySaleResponseDto, PhrPharmacySaleService>, IPhrPharmacySaleService
{
    public PhrPharmacySaleService(
        IRepository<PhrPharmacySale> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreatePharmacySaleDto>? createValidator,
        IValidator<UpdatePharmacySaleDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<PhrPharmacySaleService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<PharmacySaleResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
