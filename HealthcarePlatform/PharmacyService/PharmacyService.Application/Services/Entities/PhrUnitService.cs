using AutoMapper;
using FluentValidation;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using PharmacyService.Application.DTOs.Entities;
using PharmacyService.Application.Services.Extended;
using PharmacyService.Domain.Entities;
using PharmacyService.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace PharmacyService.Application.Services.Entities;

public interface IPhrUnitService
{
    Task<BaseResponse<UnitResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<UnitResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<UnitResponseDto>> CreateAsync(CreateUnitDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<UnitResponseDto>> UpdateAsync(long id, UpdateUnitDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class PhrUnitService
    : PhrCrudServiceBase<PhrUnit, CreateUnitDto, UpdateUnitDto, UnitResponseDto, PhrUnitService>,
        IPhrUnitService
{
    public PhrUnitService(
        IRepository<PhrUnit> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateUnitDto>? createValidator,
        IValidator<UpdateUnitDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<PhrUnitService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    protected override bool RequiresFacilityId => false;

    public Task<BaseResponse<PagedResponse<UnitResponseDto>>> GetPagedAsync(
        PagedQuery query,
        CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
