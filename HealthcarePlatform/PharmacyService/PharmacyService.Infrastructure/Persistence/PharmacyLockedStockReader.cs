using Healthcare.Common.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using PharmacyService.Application.Abstractions;
using PharmacyService.Domain.Entities;

namespace PharmacyService.Infrastructure.Persistence;

public sealed class PharmacyLockedStockReader : IPharmacyLockedStockReader
{
    private readonly PharmacyDbContext _db;
    private readonly ITenantContext _tenant;

    public PharmacyLockedStockReader(PharmacyDbContext db, ITenantContext tenant)
    {
        _db = db;
        _tenant = tenant;
    }

    public async Task<PhrBatchStock?> GetBatchStockRowLockedAsync(
        long medicineBatchId,
        long facilityId,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenant.TenantId;
        return await _db.PhrBatchStocks
            .FromSqlInterpolated($@"
                SELECT bs.*
                FROM BatchStock AS bs WITH (UPDLOCK, ROWLOCK)
                WHERE bs.MedicineBatchId = {medicineBatchId}
                  AND bs.FacilityId = {facilityId}
                  AND bs.TenantId = {tenantId}
                  AND bs.IsDeleted = 0")
            .AsTracking()
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<decimal> SumMedicineFacilityStockLockedAsync(
        long medicineId,
        long facilityId,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenant.TenantId;
        return _db.Database
            .SqlQuery<decimal>($@"
                SELECT CAST(ISNULL(SUM(bs.CurrentQty), 0) AS DECIMAL(18,4)) AS Value
                FROM BatchStock AS bs WITH (UPDLOCK, ROWLOCK)
                INNER JOIN MedicineBatch AS mb WITH (UPDLOCK, ROWLOCK) ON mb.Id = bs.MedicineBatchId
                WHERE mb.MedicineId = {medicineId}
                  AND bs.FacilityId = {facilityId}
                  AND bs.TenantId = {tenantId}
                  AND bs.IsDeleted = 0
                  AND mb.IsDeleted = 0")
            .SingleAsync(cancellationToken);
    }
}
