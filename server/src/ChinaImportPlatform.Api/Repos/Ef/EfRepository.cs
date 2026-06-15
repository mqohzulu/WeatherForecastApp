using ChinaImportPlatform.Api.Data;
using ChinaImportPlatform.Api.IRepos;
using ChinaImportPlatform.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ChinaImportPlatform.Api.Repos.Ef;

/// <summary>
/// EF Core implementation of the generic repository. Mirrors the behaviour of
/// <see cref="JsonRepository{T}"/> so services are unaffected by the swap.
///
/// Aggregates own their child collections (Order → Items/StatusHistory,
/// Product → Variants/Images, PaymentRequest → Payments) and those are mapped as
/// JSON columns, so a disconnected <c>Update</c> simply overwrites the document —
/// no child-row reconciliation is required.
/// </summary>
public abstract class EfRepository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext Context;

    protected EfRepository(AppDbContext context)
    {
        Context = context;
    }

    protected DbSet<T> Set => Context.Set<T>();

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default) =>
        await Set.AsNoTracking().ToListAsync(ct);

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await Set.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, ct);

    public virtual async Task<IReadOnlyList<T>> FindAsync(Func<T, bool> predicate, CancellationToken ct = default)
    {
        // The interface uses an in-memory predicate; concrete repositories override the
        // hot-path queries below with server-side LINQ. This fallback is rarely hit.
        var all = await Set.AsNoTracking().ToListAsync(ct);
        return all.Where(predicate).ToList();
    }

    public virtual async Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        if (entity.Id == Guid.Empty)
        {
            entity.Id = Guid.NewGuid();
        }

        if (entity.CreatedAt == default)
        {
            entity.CreatedAt = DateTime.UtcNow;
        }

        Set.Add(entity);
        await Context.SaveChangesAsync(ct);
        return entity;
    }

    public virtual async Task<bool> UpdateAsync(T entity, CancellationToken ct = default)
    {
        var exists = await Set.AsNoTracking().AnyAsync(e => e.Id == entity.Id, ct);
        if (!exists)
        {
            return false;
        }

        entity.UpdatedAt = DateTime.UtcNow;
        Set.Update(entity);
        await Context.SaveChangesAsync(ct);
        return true;
    }

    public virtual async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await Set.FirstOrDefaultAsync(e => e.Id == id, ct);
        if (entity is null)
        {
            return false;
        }

        Set.Remove(entity);
        await Context.SaveChangesAsync(ct);
        return true;
    }
}
