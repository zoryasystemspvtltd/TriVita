using HMSService.Domain.Entities;

namespace HMSService.Domain.Repositories;

public interface IVisitRepository : IRepository<HmsVisit>
{
    Task<(IReadOnlyList<HmsVisit> Items, int Total)> GetPagedAsync(
        int page,
        int pageSize,
        string? sortBy,
        bool sortDescending,
        long? patientId,
        long? doctorId,
        DateTime? visitFrom,
        DateTime? visitTo,
        long? facilityId,
        CancellationToken cancellationToken = default);
}
