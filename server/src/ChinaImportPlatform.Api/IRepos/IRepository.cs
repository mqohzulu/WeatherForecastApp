using ChinaImportPlatform.Api.Models;

namespace ChinaImportPlatform.Api.IRepos;

/// <summary>
/// Generic persistence contract for an aggregate root. The current implementation
/// is JSON-file backed; swapping in an EF Core implementation later requires no
/// change to the services that depend on this interface.
/// </summary>
public interface IRepository<T> where T : BaseEntity
{
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);

    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<IReadOnlyList<T>> FindAsync(Func<T, bool> predicate, CancellationToken ct = default);

    Task<T> AddAsync(T entity, CancellationToken ct = default);

    Task<bool> UpdateAsync(T entity, CancellationToken ct = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
