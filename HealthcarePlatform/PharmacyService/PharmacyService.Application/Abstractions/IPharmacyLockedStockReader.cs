using PharmacyService.Domain.Entities;

namespace PharmacyService.Application.Abstractions;

public interface IPharmacyLockedStockReader
{
    Task<PhrBatchStock?> GetBatchStockRowLockedAsync(long medicineBatchId, long facilityId, CancellationToken cancellationToken);

    Task<decimal> SumMedicineFacilityStockLockedAsync(long medicineId, long facilityId, CancellationToken cancellationToken);
}
