using CommunicationService.Domain.Entities;

namespace CommunicationService.Domain.Repositories;

public interface INotificationQueueRepository
{
    Task<IReadOnlyList<ComNotificationQueue>> GetPendingBatchAsync(int take, CancellationToken cancellationToken = default);

    Task<ComNotificationQueue?> GetByIdIgnoreFiltersAsync(long id, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
