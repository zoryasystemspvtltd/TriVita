using HMSService.Domain.Entities;
using HMSService.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HMSService.Infrastructure.Persistence.Repositories;

public sealed class VisitRepository : EfRepository<HmsVisit>, IVisitRepository
{
    public VisitRepository(HmsDbContext db)
        : base(db)
    {
    }

    public async Task<(IReadOnlyList<HmsVisit> Items, int Total)> GetPagedAsync(
        int page,
        int pageSize,
        string? sortBy,
        bool sortDescending,
        long? patientId,
        long? doctorId,
        DateTime? visitFrom,
        DateTime? visitTo,
        long? facilityId,
        CancellationToken cancellationToken = default)
    {
        var query = Db.Visits.AsNoTracking().AsQueryable();

        if (facilityId is not null)
            query = query.Where(v => v.FacilityId == facilityId);

        if (patientId is not null)
            query = query.Where(v => v.PatientId == patientId);

        if (doctorId is not null)
            query = query.Where(v => v.DoctorId == doctorId);

        if (visitFrom is not null)
            query = query.Where(v => v.VisitStartOn >= visitFrom);

        if (visitTo is not null)
            query = query.Where(v => v.VisitStartOn <= visitTo);

        var total = await query.CountAsync(cancellationToken);

        query = ApplySorting(query, sortBy, sortDescending);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    private static IQueryable<HmsVisit> ApplySorting(
        IQueryable<HmsVisit> query,
        string? sortBy,
        bool sortDescending)
    {
        return sortBy?.ToLowerInvariant() switch
        {
            "visitno" => sortDescending
                ? query.OrderByDescending(v => v.VisitNo)
                : query.OrderBy(v => v.VisitNo),
            "createdon" => sortDescending
                ? query.OrderByDescending(v => v.CreatedOn)
                : query.OrderBy(v => v.CreatedOn),
            "visitstarton" or _ => sortDescending
                ? query.OrderByDescending(v => v.VisitStartOn)
                : query.OrderBy(v => v.VisitStartOn)
        };
    }
}
