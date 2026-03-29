using HMSService.Domain.Entities;

namespace HMSService.Domain.Repositories;

public interface IAppointmentRepository : IRepository<HmsAppointment>
{
    Task<(IReadOnlyList<HmsAppointment> Items, int Total)> GetPagedAsync(
        int page,
        int pageSize,
        string? sortBy,
        bool sortDescending,
        long? patientId,
        long? doctorId,
        DateTime? scheduledFrom,
        DateTime? scheduledTo,
        long? facilityId,
        CancellationToken cancellationToken = default);
}
