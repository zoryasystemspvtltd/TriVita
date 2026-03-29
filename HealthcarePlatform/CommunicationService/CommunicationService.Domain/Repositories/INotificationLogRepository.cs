using CommunicationService.Domain.Entities;

namespace CommunicationService.Domain.Repositories;

public interface INotificationLogRepository
{
    Task<(IReadOnlyList<ComNotificationLog> Items, int TotalCount)> GetPagedAsync(
        long? notificationChannelId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
