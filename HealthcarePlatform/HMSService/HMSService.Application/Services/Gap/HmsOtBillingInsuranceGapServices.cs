using System.Linq.Expressions;
using AutoMapper;
using FluentValidation;
using Healthcare.Common.Integration.SharedService;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using HMSService.Application.DTOs.Gap;
using HMSService.Application.Services.Extended;
using HMSService.Domain.Entities;
using HMSService.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace HMSService.Application.Services.Gap;

public sealed class OperationTheatreService
    : HmsCrudServiceBase<HmsOperationTheatre, CreateOperationTheatreDto, UpdateOperationTheatreDto, OperationTheatreResponseDto, OperationTheatreService>,
        IOperationTheatreService
{
    public OperationTheatreService(
        IRepository<HmsOperationTheatre> repository,
        IMapper mapper,
        ITenantContext tenant,
        IValidator<CreateOperationTheatreDto>? createValidator,
        IValidator<UpdateOperationTheatreDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<OperationTheatreService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    public Task<BaseResponse<PagedResponse<OperationTheatreResponseDto>>> GetPagedAsync(
        PagedQuery query,
        CancellationToken cancellationToken = default) =>
        GetPagedCoreAsync(query, null, cancellationToken);
}

public sealed class SurgeryScheduleService
    : HmsCrudServiceBase<HmsSurgerySchedule, CreateSurgeryScheduleDto, UpdateSurgeryScheduleDto, SurgeryScheduleResponseDto, SurgeryScheduleService>,
        ISurgeryScheduleService
{
    public SurgeryScheduleService(
        IRepository<HmsSurgerySchedule> repository,
        IMapper mapper,
        ITenantContext tenant,
        IValidator<CreateSurgeryScheduleDto>? createValidator,
        IValidator<UpdateSurgeryScheduleDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<SurgeryScheduleService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    public Task<BaseResponse<PagedResponse<SurgeryScheduleResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? operationTheatreId,
        long? patientMasterId,
        CancellationToken cancellationToken = default)
    {
        Expression<Func<HmsSurgerySchedule, bool>>? pred = null;
        if (operationTheatreId is long ot)
            pred = e => e.OperationTheatreId == ot;
        if (patientMasterId is long pm)
        {
            Expression<Func<HmsSurgerySchedule, bool>> p = e => e.PatientMasterId == pm;
            pred = pred is null ? p : AndAlso(pred, p);
        }

        return GetPagedCoreAsync(query, pred, cancellationToken);
    }

    private static Expression<Func<HmsSurgerySchedule, bool>> AndAlso(
        Expression<Func<HmsSurgerySchedule, bool>> first,
        Expression<Func<HmsSurgerySchedule, bool>> second)
    {
        var param = Expression.Parameter(typeof(HmsSurgerySchedule), "e");
        var left = new HmsSurgeryParamVisitor(first.Parameters[0], param).Visit(first.Body);
        var right = new HmsSurgeryParamVisitor(second.Parameters[0], param).Visit(second.Body);
        return Expression.Lambda<Func<HmsSurgerySchedule, bool>>(Expression.AndAlso(left!, right!), param);
    }

    private sealed class HmsSurgeryParamVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _from;
        private readonly ParameterExpression _to;

        public HmsSurgeryParamVisitor(ParameterExpression from, ParameterExpression to)
        {
            _from = from;
            _to = to;
        }

        protected override Expression VisitParameter(ParameterExpression node) =>
            node == _from ? _to : base.VisitParameter(node);
    }
}

public sealed class AnesthesiaRecordService
    : HmsCrudServiceBase<HmsAnesthesiaRecord, CreateAnesthesiaRecordDto, UpdateAnesthesiaRecordDto, AnesthesiaRecordResponseDto, AnesthesiaRecordService>,
        IAnesthesiaRecordService
{
    public AnesthesiaRecordService(
        IRepository<HmsAnesthesiaRecord> repository,
        IMapper mapper,
        ITenantContext tenant,
        IValidator<CreateAnesthesiaRecordDto>? createValidator,
        IValidator<UpdateAnesthesiaRecordDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<AnesthesiaRecordService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    public Task<BaseResponse<PagedResponse<AnesthesiaRecordResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? surgeryScheduleId,
        CancellationToken cancellationToken = default) =>
        GetPagedCoreAsync(
            query,
            surgeryScheduleId is null ? null : e => e.SurgeryScheduleId == surgeryScheduleId.Value,
            cancellationToken);
}

public sealed class PostOpRecordService
    : HmsCrudServiceBase<HmsPostOpRecord, CreatePostOpRecordDto, UpdatePostOpRecordDto, PostOpRecordResponseDto, PostOpRecordService>,
        IPostOpRecordService
{
    public PostOpRecordService(
        IRepository<HmsPostOpRecord> repository,
        IMapper mapper,
        ITenantContext tenant,
        IValidator<CreatePostOpRecordDto>? createValidator,
        IValidator<UpdatePostOpRecordDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<PostOpRecordService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    public Task<BaseResponse<PagedResponse<PostOpRecordResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? surgeryScheduleId,
        CancellationToken cancellationToken = default) =>
        GetPagedCoreAsync(
            query,
            surgeryScheduleId is null ? null : e => e.SurgeryScheduleId == surgeryScheduleId.Value,
            cancellationToken);
}

public sealed class OtConsumableService
    : HmsCrudServiceBase<HmsOtConsumable, CreateOtConsumableDto, UpdateOtConsumableDto, OtConsumableResponseDto, OtConsumableService>,
        IOtConsumableService
{
    public OtConsumableService(
        IRepository<HmsOtConsumable> repository,
        IMapper mapper,
        ITenantContext tenant,
        IValidator<CreateOtConsumableDto>? createValidator,
        IValidator<UpdateOtConsumableDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<OtConsumableService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    public Task<BaseResponse<PagedResponse<OtConsumableResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? surgeryScheduleId,
        CancellationToken cancellationToken = default) =>
        GetPagedCoreAsync(
            query,
            surgeryScheduleId is null ? null : e => e.SurgeryScheduleId == surgeryScheduleId.Value,
            cancellationToken);
}

public sealed class PricingRuleService
    : HmsCrudServiceBase<HmsPricingRule, CreatePricingRuleDto, UpdatePricingRuleDto, PricingRuleResponseDto, PricingRuleService>,
        IPricingRuleService
{
    public PricingRuleService(
        IRepository<HmsPricingRule> repository,
        IMapper mapper,
        ITenantContext tenant,
        IValidator<CreatePricingRuleDto>? createValidator,
        IValidator<UpdatePricingRuleDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<PricingRuleService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    public Task<BaseResponse<PagedResponse<PricingRuleResponseDto>>> GetPagedAsync(
        PagedQuery query,
        CancellationToken cancellationToken = default) =>
        GetPagedCoreAsync(query, null, cancellationToken);
}

public sealed class PackageDefinitionService
    : HmsCrudServiceBase<HmsPackageDefinition, CreatePackageDefinitionDto, UpdatePackageDefinitionDto, PackageDefinitionResponseDto, PackageDefinitionService>,
        IPackageDefinitionService
{
    public PackageDefinitionService(
        IRepository<HmsPackageDefinition> repository,
        IMapper mapper,
        ITenantContext tenant,
        IValidator<CreatePackageDefinitionDto>? createValidator,
        IValidator<UpdatePackageDefinitionDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<PackageDefinitionService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    public Task<BaseResponse<PagedResponse<PackageDefinitionResponseDto>>> GetPagedAsync(
        PagedQuery query,
        CancellationToken cancellationToken = default) =>
        GetPagedCoreAsync(query, null, cancellationToken);
}

public sealed class PackageDefinitionLineService
    : HmsCrudServiceBase<HmsPackageDefinitionLine, CreatePackageDefinitionLineDto, UpdatePackageDefinitionLineDto, PackageDefinitionLineResponseDto, PackageDefinitionLineService>,
        IPackageDefinitionLineService
{
    private readonly IRepository<HmsPackageDefinition> _packages;

    public PackageDefinitionLineService(
        IRepository<HmsPackageDefinitionLine> repository,
        IRepository<HmsPackageDefinition> packages,
        IMapper mapper,
        ITenantContext tenant,
        IValidator<CreatePackageDefinitionLineDto>? createValidator,
        IValidator<UpdatePackageDefinitionLineDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<PackageDefinitionLineService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
        _packages = packages;
    }

    public override async Task<BaseResponse<PackageDefinitionLineResponseDto>> CreateAsync(
        CreatePackageDefinitionLineDto dto,
        CancellationToken cancellationToken = default)
    {
        if (Tenant.FacilityId is null)
            return BaseResponse<PackageDefinitionLineResponseDto>.Fail("FacilityId is required (header X-Facility-Id or claim facility_id).");

        var pkg = await _packages.GetByIdAsync(dto.PackageDefinitionId, cancellationToken);
        if (pkg is null || pkg.FacilityId != Tenant.FacilityId)
            return BaseResponse<PackageDefinitionLineResponseDto>.Fail("Package definition not found in the current facility.");

        return await base.CreateAsync(dto, cancellationToken);
    }

    public Task<BaseResponse<PagedResponse<PackageDefinitionLineResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? packageDefinitionId,
        CancellationToken cancellationToken = default) =>
        GetPagedCoreAsync(
            query,
            packageDefinitionId is null ? null : e => e.PackageDefinitionId == packageDefinitionId.Value,
            cancellationToken);
}

public sealed class ProformaInvoiceService
    : HmsCrudServiceBase<HmsProformaInvoice, CreateProformaInvoiceDto, UpdateProformaInvoiceDto, ProformaInvoiceResponseDto, ProformaInvoiceService>,
        IProformaInvoiceService
{
    public ProformaInvoiceService(
        IRepository<HmsProformaInvoice> repository,
        IMapper mapper,
        ITenantContext tenant,
        IValidator<CreateProformaInvoiceDto>? createValidator,
        IValidator<UpdateProformaInvoiceDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<ProformaInvoiceService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    protected override Task OnBeforeCreateAsync(
        HmsProformaInvoice entity,
        CreateProformaInvoiceDto dto,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(entity.ProformaNo))
            entity.ProformaNo = HmsDocumentNumberHelper.Generate("PROF");
        return Task.CompletedTask;
    }

    public Task<BaseResponse<PagedResponse<ProformaInvoiceResponseDto>>> GetPagedAsync(
        PagedQuery query,
        CancellationToken cancellationToken = default) =>
        GetPagedCoreAsync(query, null, cancellationToken);
}

public sealed class InsuranceProviderService
    : HmsCrudServiceBase<HmsInsuranceProvider, CreateInsuranceProviderDto, UpdateInsuranceProviderDto, InsuranceProviderResponseDto, InsuranceProviderService>,
        IInsuranceProviderService
{
    public InsuranceProviderService(
        IRepository<HmsInsuranceProvider> repository,
        IMapper mapper,
        ITenantContext tenant,
        IValidator<CreateInsuranceProviderDto>? createValidator,
        IValidator<UpdateInsuranceProviderDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<InsuranceProviderService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    protected override bool RequiresFacilityId => false;

    public Task<BaseResponse<PagedResponse<InsuranceProviderResponseDto>>> GetPagedAsync(
        PagedQuery query,
        CancellationToken cancellationToken = default) =>
        GetPagedCoreAsync(query, null, cancellationToken);
}

public sealed class PreAuthorizationService
    : HmsCrudServiceBase<HmsPreAuthorization, CreatePreAuthorizationDto, UpdatePreAuthorizationDto, PreAuthorizationResponseDto, PreAuthorizationService>,
        IPreAuthorizationService
{
    public PreAuthorizationService(
        IRepository<HmsPreAuthorization> repository,
        IMapper mapper,
        ITenantContext tenant,
        IValidator<CreatePreAuthorizationDto>? createValidator,
        IValidator<UpdatePreAuthorizationDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<PreAuthorizationService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    protected override Task OnBeforeCreateAsync(
        HmsPreAuthorization entity,
        CreatePreAuthorizationDto dto,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(entity.PreAuthNo))
            entity.PreAuthNo = HmsDocumentNumberHelper.Generate("PAUTH");
        return Task.CompletedTask;
    }

    public Task<BaseResponse<PagedResponse<PreAuthorizationResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? insuranceProviderId,
        long? patientMasterId,
        CancellationToken cancellationToken = default)
    {
        Expression<Func<HmsPreAuthorization, bool>>? pred = null;
        if (insuranceProviderId is long ip)
            pred = e => e.InsuranceProviderId == ip;
        if (patientMasterId is long pm)
        {
            Expression<Func<HmsPreAuthorization, bool>> p = e => e.PatientMasterId == pm;
            pred = pred is null ? p : PreAuthAnd(pred, p);
        }

        return GetPagedCoreAsync(query, pred, cancellationToken);
    }

    private static Expression<Func<HmsPreAuthorization, bool>> PreAuthAnd(
        Expression<Func<HmsPreAuthorization, bool>> first,
        Expression<Func<HmsPreAuthorization, bool>> second)
    {
        var param = Expression.Parameter(typeof(HmsPreAuthorization), "e");
        var left = new PreAuthVisitor(first.Parameters[0], param).Visit(first.Body);
        var right = new PreAuthVisitor(second.Parameters[0], param).Visit(second.Body);
        return Expression.Lambda<Func<HmsPreAuthorization, bool>>(Expression.AndAlso(left!, right!), param);
    }

    private sealed class PreAuthVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _from;
        private readonly ParameterExpression _to;

        public PreAuthVisitor(ParameterExpression from, ParameterExpression to)
        {
            _from = from;
            _to = to;
        }

        protected override Expression VisitParameter(ParameterExpression node) =>
            node == _from ? _to : base.VisitParameter(node);
    }
}

public sealed class ClaimService
    : HmsCrudServiceBase<HmsClaim, CreateClaimDto, UpdateClaimDto, ClaimResponseDto, ClaimService>,
        IClaimService
{
    public ClaimService(
        IRepository<HmsClaim> repository,
        IMapper mapper,
        ITenantContext tenant,
        IValidator<CreateClaimDto>? createValidator,
        IValidator<UpdateClaimDto>? updateValidator,
        IFacilityTenantValidator facilityValidator,
        ILogger<ClaimService> logger)
        : base(repository, mapper, tenant, createValidator, updateValidator, facilityValidator, logger)
    {
    }

    protected override Task OnBeforeCreateAsync(
        HmsClaim entity,
        CreateClaimDto dto,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(entity.ClaimNo))
            entity.ClaimNo = HmsDocumentNumberHelper.Generate("CLM");
        return Task.CompletedTask;
    }

    public Task<BaseResponse<PagedResponse<ClaimResponseDto>>> GetPagedAsync(
        PagedQuery query,
        long? insuranceProviderId,
        long? patientMasterId,
        CancellationToken cancellationToken = default)
    {
        Expression<Func<HmsClaim, bool>>? pred = null;
        if (insuranceProviderId is long ip)
            pred = e => e.InsuranceProviderId == ip;
        if (patientMasterId is long pm)
        {
            Expression<Func<HmsClaim, bool>> p = e => e.PatientMasterId == pm;
            pred = pred is null ? p : ClaimAnd(pred, p);
        }

        return GetPagedCoreAsync(query, pred, cancellationToken);
    }

    private static Expression<Func<HmsClaim, bool>> ClaimAnd(
        Expression<Func<HmsClaim, bool>> first,
        Expression<Func<HmsClaim, bool>> second)
    {
        var param = Expression.Parameter(typeof(HmsClaim), "e");
        var left = new ClaimVisitor(first.Parameters[0], param).Visit(first.Body);
        var right = new ClaimVisitor(second.Parameters[0], param).Visit(second.Body);
        return Expression.Lambda<Func<HmsClaim, bool>>(Expression.AndAlso(left!, right!), param);
    }

    private sealed class ClaimVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _from;
        private readonly ParameterExpression _to;

        public ClaimVisitor(ParameterExpression from, ParameterExpression to)
        {
            _from = from;
            _to = to;
        }

        protected override Expression VisitParameter(ParameterExpression node) =>
            node == _from ? _to : base.VisitParameter(node);
    }
}
