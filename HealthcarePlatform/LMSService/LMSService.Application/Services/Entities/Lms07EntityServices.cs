using AutoMapper;
using FluentValidation;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using LMSService.Application.DTOs.Entities;
using LMSService.Application.Services.Extended;
using LMSService.Domain.Entities;
using LMSService.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace LMSService.Application.Services.Entities;

public interface ILmsLabInvoiceHeaderService
{
    Task<BaseResponse<LabInvoiceHeaderResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<LabInvoiceHeaderResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<LabInvoiceHeaderResponseDto>> CreateAsync(CreateLabInvoiceHeaderDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<LabInvoiceHeaderResponseDto>> UpdateAsync(long id, UpdateLabInvoiceHeaderDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsLabInvoiceHeaderService
    : LmsCrudServiceBase<LmsLabInvoiceHeader, CreateLabInvoiceHeaderDto, UpdateLabInvoiceHeaderDto, LabInvoiceHeaderResponseDto, LmsLabInvoiceHeaderService>,
        ILmsLabInvoiceHeaderService
{
    public LmsLabInvoiceHeaderService(
        IRepository<LmsLabInvoiceHeader> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateLabInvoiceHeaderDto>? createValidator,
        IValidator<UpdateLabInvoiceHeaderDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<LmsLabInvoiceHeaderService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    public Task<BaseResponse<PagedResponse<LabInvoiceHeaderResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}

public interface ILmsLabOrderContextService
{
    Task<BaseResponse<LabOrderContextResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<LabOrderContextResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<LabOrderContextResponseDto>> CreateAsync(CreateLabOrderContextDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<LabOrderContextResponseDto>> UpdateAsync(long id, UpdateLabOrderContextDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsLabOrderContextService
    : LmsCrudServiceBase<LmsLabOrderContext, CreateLabOrderContextDto, UpdateLabOrderContextDto, LabOrderContextResponseDto, LmsLabOrderContextService>,
        ILmsLabOrderContextService
{
    public LmsLabOrderContextService(
        IRepository<LmsLabOrderContext> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateLabOrderContextDto>? createValidator,
        IValidator<UpdateLabOrderContextDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<LmsLabOrderContextService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    public Task<BaseResponse<PagedResponse<LabOrderContextResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}

public interface ILmsTestPackageService
{
    Task<BaseResponse<TestPackageResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<TestPackageResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<TestPackageResponseDto>> CreateAsync(CreateTestPackageDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<TestPackageResponseDto>> UpdateAsync(long id, UpdateTestPackageDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsTestPackageService
    : LmsCrudServiceBase<LmsTestPackage, CreateTestPackageDto, UpdateTestPackageDto, TestPackageResponseDto, LmsTestPackageService>,
        ILmsTestPackageService
{
    public LmsTestPackageService(
        IRepository<LmsTestPackage> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateTestPackageDto>? createValidator,
        IValidator<UpdateTestPackageDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<LmsTestPackageService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    public Task<BaseResponse<PagedResponse<TestPackageResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}

public interface IIamUserAccountService
{
    Task<BaseResponse<IamUserAccountResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<IamUserAccountResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<IamUserAccountResponseDto>> CreateAsync(CreateIamUserAccountDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<IamUserAccountResponseDto>> UpdateAsync(long id, UpdateIamUserAccountDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class IamUserAccountService
    : LmsCrudServiceBase<IamUserAccount, CreateIamUserAccountDto, UpdateIamUserAccountDto, IamUserAccountResponseDto, IamUserAccountService>,
        IIamUserAccountService
{
    public IamUserAccountService(
        IRepository<IamUserAccount> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateIamUserAccountDto>? createValidator,
        IValidator<UpdateIamUserAccountDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<IamUserAccountService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    protected override bool RequiresFacilityId => false;

    public Task<BaseResponse<PagedResponse<IamUserAccountResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}

public interface ILmsReagentConsumptionLogService
{
    Task<BaseResponse<ReagentConsumptionLogResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<ReagentConsumptionLogResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<ReagentConsumptionLogResponseDto>> CreateAsync(CreateReagentConsumptionLogDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<ReagentConsumptionLogResponseDto>> UpdateAsync(long id, UpdateReagentConsumptionLogDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class LmsReagentConsumptionLogService
    : LmsCrudServiceBase<LmsReagentConsumptionLog, CreateReagentConsumptionLogDto, UpdateReagentConsumptionLogDto, ReagentConsumptionLogResponseDto, LmsReagentConsumptionLogService>,
        ILmsReagentConsumptionLogService
{
    public LmsReagentConsumptionLogService(
        IRepository<LmsReagentConsumptionLog> repository,
        IMapper mapper,
        Healthcare.Common.MultiTenancy.ITenantContext tenant,
        IValidator<CreateReagentConsumptionLogDto>? createValidator,
        IValidator<UpdateReagentConsumptionLogDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<LmsReagentConsumptionLogService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    public Task<BaseResponse<PagedResponse<ReagentConsumptionLogResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
        => GetPagedCoreAsync(query, null, cancellationToken);
}
