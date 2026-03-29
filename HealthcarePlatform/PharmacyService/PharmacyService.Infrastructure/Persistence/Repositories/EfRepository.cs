using Healthcare.Common.Entities;
using PharmacyService.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace PharmacyService.Infrastructure.Persistence.Repositories;

public class EfRepository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly PharmacyDbContext Db;

    public EfRepository(PharmacyDbContext db)
    {
        Db = db;
    }

    public virtual async Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        await Db.Set<T>().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public virtual async Task<IReadOnlyList<T>> ListAsync(
        System.Linq.Expressions.Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        var query = Db.Set<T>().AsQueryable();
        if (predicate is not null)
            query = query.Where(predicate);
        return await query.ToListAsync(cancellationToken);
    }

    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await Db.Set<T>().AddAsync(entity, cancellationToken);
    }

    public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        Db.Set<T>().Update(entity);
        return Task.CompletedTask;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        Db.SaveChangesAsync(cancellationToken);

    public virtual async Task<(IReadOnlyList<T> Items, int Total)> GetPagedByFilterAsync(
        int page,
        int pageSize,
        System.Linq.Expressions.Expression<Func<T, bool>>? predicate,
        CancellationToken cancellationToken = default)
    {
        var query = Db.Set<T>().AsQueryable();
        if (predicate is not null)
            query = query.Where(predicate);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(e => e.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }
}
