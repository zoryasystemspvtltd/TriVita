using CommunicationService.Application.Options;
using CommunicationService.Domain.Entities;
using CommunicationService.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CommunicationService.Infrastructure.Persistence.Repositories;

public sealed class NotificationQueueRepository : INotificationQueueRepository
{
    private readonly CommunicationDbContext _db;
    private readonly CommunicationOptions _options;

    public NotificationQueueRepository(CommunicationDbContext db, IOptions<CommunicationOptions> options)
    {
        _db = db;
        _options = options.Value;
    }

    public async Task<IReadOnlyList<ComNotificationQueue>> GetPendingBatchAsync(int take, CancellationToken cancellationToken = default)
    {
        var pending = _options.ReferenceValueIds.QueuePending;
        var now = DateTime.UtcNow;

        return await _db.NotificationQueues
            .IgnoreQueryFilters()
            .Where(q => !q.IsDeleted
                        && q.StatusReferenceValueId == pending
                        && q.ScheduledOn <= now)
            .OrderBy(q => q.ScheduledOn)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<ComNotificationQueue?> GetByIdIgnoreFiltersAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _db.NotificationQueues
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _db.SaveChangesAsync(cancellationToken);
}
