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

public interface IPhrMedicineBatchService
{
    Task<BaseResponse<MedicineBatchResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<MedicineBatchResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<MedicineBatchResponseDto>> CreateAsync(CreateMedicineBatchDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<MedicineBatchResponseDto>> UpdateAsync(long id, UpdateMedicineBatchDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class PhrMedicineBatchService : PhrCrudServiceBase<PhrMedicineBatch, CreateMedicineBatchDto, UpdateMedicineBatchDto, MedicineBatchResponseDto, PhrMedicineBatchService>, IPhrMedicineBatchService
{
    public PhrMedicineBatchService(
        IRepository<PhrMedicineBatch> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateMedicineBatchDto>? createValidator,
        IValidator<UpdateMedicineBatchDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<PhrMedicineBatchService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => false;

    public Task<BaseResponse<PagedResponse<MedicineBatchResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
