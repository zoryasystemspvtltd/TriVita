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

public interface IPhrPrescriptionMappingService
{
    Task<BaseResponse<PrescriptionMappingResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<PrescriptionMappingResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<PrescriptionMappingResponseDto>> CreateAsync(CreatePrescriptionMappingDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<PrescriptionMappingResponseDto>> UpdateAsync(long id, UpdatePrescriptionMappingDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class PhrPrescriptionMappingService : PhrCrudServiceBase<PhrPrescriptionMapping, CreatePrescriptionMappingDto, UpdatePrescriptionMappingDto, PrescriptionMappingResponseDto, PhrPrescriptionMappingService>, IPhrPrescriptionMappingService
{
    public PhrPrescriptionMappingService(
        IRepository<PhrPrescriptionMapping> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreatePrescriptionMappingDto>? createValidator,
        IValidator<UpdatePrescriptionMappingDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<PhrPrescriptionMappingService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<PrescriptionMappingResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
