using System.Linq.Expressions;
using Healthcare.Common.Entities;

namespace PharmacyService.Domain.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);

    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<T> Items, int Total)> GetPagedByFilterAsync(
        int page,
        int pageSize,
        Expression<Func<T, bool>>? predicate,
        CancellationToken cancellationToken = default);
}
