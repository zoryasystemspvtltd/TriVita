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

public interface IPhrMedicineCategoryService
{
    Task<BaseResponse<MedicineCategoryResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<MedicineCategoryResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<MedicineCategoryResponseDto>> CreateAsync(CreateMedicineCategoryDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<MedicineCategoryResponseDto>> UpdateAsync(long id, UpdateMedicineCategoryDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class PhrMedicineCategoryService : PhrCrudServiceBase<PhrMedicineCategory, CreateMedicineCategoryDto, UpdateMedicineCategoryDto, MedicineCategoryResponseDto, PhrMedicineCategoryService>, IPhrMedicineCategoryService
{
    public PhrMedicineCategoryService(
        IRepository<PhrMedicineCategory> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateMedicineCategoryDto>? createValidator,
        IValidator<UpdateMedicineCategoryDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<PhrMedicineCategoryService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => false;

    public Task<BaseResponse<PagedResponse<MedicineCategoryResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
