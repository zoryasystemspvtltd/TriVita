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
}
