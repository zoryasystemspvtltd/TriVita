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

public interface IPhrMedicineCompositionService
{
    Task<BaseResponse<MedicineCompositionResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<MedicineCompositionResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<MedicineCompositionResponseDto>> CreateAsync(CreateMedicineCompositionDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<MedicineCompositionResponseDto>> UpdateAsync(long id, UpdateMedicineCompositionDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class PhrMedicineCompositionService : PhrCrudServiceBase<PhrMedicineComposition, CreateMedicineCompositionDto, UpdateMedicineCompositionDto, MedicineCompositionResponseDto, PhrMedicineCompositionService>, IPhrMedicineCompositionService
{
    public PhrMedicineCompositionService(
        IRepository<PhrMedicineComposition> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateMedicineCompositionDto>? createValidator,
        IValidator<UpdateMedicineCompositionDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<PhrMedicineCompositionService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => false;

    public Task<BaseResponse<PagedResponse<MedicineCompositionResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
