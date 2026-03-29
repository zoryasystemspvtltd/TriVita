using AutoMapper;
using FluentValidation;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using LMSService.Domain.Entities;
using LMSService.Application.DTOs.Entities;
using LMSService.Domain.Repositories;
using LMSService.Application.Services.Extended;
using Microsoft.Extensions.Logging;

namespace LMSService.Application.Services.Entities;

public interface ILmsLabInventoryTransactionService
{
    Task<BaseResponse<LabInventoryTransactionResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<LabInventoryTransactionResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<LabInventoryTransactionResponseDto>> CreateAsync(CreateLabInventoryTransactionDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<LabInventoryTransactionResponseDto>> UpdateAsync(long id, UpdateLabInventoryTransactionDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsLabInventoryTransactionService : LmsCrudServiceBase<LmsLabInventoryTransaction, CreateLabInventoryTransactionDto, UpdateLabInventoryTransactionDto, LabInventoryTransactionResponseDto, LmsLabInventoryTransactionService>, ILmsLabInventoryTransactionService
{
    public LmsLabInventoryTransactionService(
        IRepository<LmsLabInventoryTransaction> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateLabInventoryTransactionDto>? createValidator,
        IValidator<UpdateLabInventoryTransactionDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<LmsLabInventoryTransactionService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger) { }

    protected override bool RequiresFacilityId => true;

    public Task<BaseResponse<PagedResponse<LabInventoryTransactionResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
