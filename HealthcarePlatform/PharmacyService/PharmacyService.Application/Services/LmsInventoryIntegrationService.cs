using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Responses;
using Microsoft.Extensions.Logging;

namespace PharmacyService.Application.Services;

public interface ILmsInventoryIntegrationService
{
    /// <summary>
    /// Records cross-module intent when LMS consumes inventory-linked material (e.g. reagent batch).
    /// Extend with StockLedger / batch updates when pharmacy stock is physically shared.
    /// </summary>
    Task<BaseResponse<object?>> RecordLmsReagentConsumptionAsync(
        RecordLmsReagentConsumptionRequest request,
        CancellationToken cancellationToken = default);
}

public sealed record RecordLmsReagentConsumptionRequest(
    long? LmsReagentBatchId,
    long? PharmacyMedicineBatchId,
    decimal QuantityConsumed,
    string? SourceReference,
    string? Notes);

public sealed class LmsInventoryIntegrationService : ILmsInventoryIntegrationService
{
    private readonly ITenantContext _tenant;
    private readonly ILogger<LmsInventoryIntegrationService> _logger;

    public LmsInventoryIntegrationService(ITenantContext tenant, ILogger<LmsInventoryIntegrationService> logger)
    {
        _tenant = tenant;
        _logger = logger;
    }

    public Task<BaseResponse<object?>> RecordLmsReagentConsumptionAsync(
        RecordLmsReagentConsumptionRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "LMS→Pharmacy consumption recorded tenant {TenantId} facility {FacilityId} LmsBatch {LmsBatchId} PhrBatch {PhrBatchId} qty {Qty} ref {Ref}",
            _tenant.TenantId,
            _tenant.FacilityId,
            request.LmsReagentBatchId,
            request.PharmacyMedicineBatchId,
            request.QuantityConsumed,
            request.SourceReference);

        return Task.FromResult(BaseResponse<object?>.Ok(null, "Acknowledged; extend with stock ledger when batches are linked."));
    }
}
