using CommunicationService.Domain.Entities;

namespace CommunicationService.Domain.Repositories;

public interface INotificationRepository
{
    Task<ComNotification?> GetByIdWithDetailsAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>Background worker: loads by tenant/facility with query filters disabled.</summary>
    Task<ComNotification?> GetByIdForProcessingAsync(
        long tenantId,
        long facilityId,
        long id,
        CancellationToken cancellationToken = default);

    Task AddAsync(ComNotification entity, CancellationToken cancellationToken = default);

    Task AddNotificationLogAsync(ComNotificationLog log, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
