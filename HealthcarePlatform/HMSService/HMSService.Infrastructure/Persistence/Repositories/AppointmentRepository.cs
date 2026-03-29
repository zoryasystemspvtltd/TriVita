using HMSService.Domain.Entities;
using HMSService.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HMSService.Infrastructure.Persistence.Repositories;

public sealed class AppointmentRepository : EfRepository<HmsAppointment>, IAppointmentRepository
{
    public AppointmentRepository(HmsDbContext db)
        : base(db)
    {
    }

    public async Task<(IReadOnlyList<HmsAppointment> Items, int Total)> GetPagedAsync(
        int page,
        int pageSize,
        string? sortBy,
        bool sortDescending,
        long? patientId,
        long? doctorId,
        DateTime? scheduledFrom,
        DateTime? scheduledTo,
        long? facilityId,
        CancellationToken cancellationToken = default)
    {
        var query = Db.Appointments.AsNoTracking().AsQueryable();

        if (facilityId is not null)
            query = query.Where(a => a.FacilityId == facilityId);

        if (patientId is not null)
            query = query.Where(a => a.PatientId == patientId);

        if (doctorId is not null)
            query = query.Where(a => a.DoctorId == doctorId);

        if (scheduledFrom is not null)
            query = query.Where(a => a.ScheduledStartOn >= scheduledFrom);

        if (scheduledTo is not null)
            query = query.Where(a => a.ScheduledStartOn <= scheduledTo);

        var total = await query.CountAsync(cancellationToken);

        query = ApplySorting(query, sortBy, sortDescending);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    private static IQueryable<HmsAppointment> ApplySorting(
        IQueryable<HmsAppointment> query,
        string? sortBy,
        bool sortDescending)
    {
        return sortBy?.ToLowerInvariant() switch
        {
            "appointmentno" => sortDescending
                ? query.OrderByDescending(a => a.AppointmentNo)
                : query.OrderBy(a => a.AppointmentNo),
            "createdon" => sortDescending
                ? query.OrderByDescending(a => a.CreatedOn)
                : query.OrderBy(a => a.CreatedOn),
            "scheduledstarton" or _ => sortDescending
                ? query.OrderByDescending(a => a.ScheduledStartOn)
                : query.OrderBy(a => a.ScheduledStartOn)
        };
    }
}
