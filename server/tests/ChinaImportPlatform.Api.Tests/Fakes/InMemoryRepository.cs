using ChinaImportPlatform.Api.IRepos;
using ChinaImportPlatform.Api.Models;

namespace ChinaImportPlatform.Api.Tests.Fakes;

/// <summary>A simple list-backed repository used to test services without files or a database.</summary>
public class InMemoryRepository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly List<T> Items = new();

    public InMemoryRepository(IEnumerable<T>? seed = null)
    {
        if (seed != null)
        {
            Items.AddRange(seed);
        }
    }

    public Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<T>>(Items.ToList());

    public Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        Task.FromResult(Items.FirstOrDefault(e => e.Id == id));

    public Task<IReadOnlyList<T>> FindAsync(Func<T, bool> predicate, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<T>>(Items.Where(predicate).ToList());

    public Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        if (entity.Id == Guid.Empty)
        {
            entity.Id = Guid.NewGuid();
        }

        Items.Add(entity);
        return Task.FromResult(entity);
    }

    public Task<bool> UpdateAsync(T entity, CancellationToken ct = default)
    {
        var index = Items.FindIndex(e => e.Id == entity.Id);
        if (index < 0)
        {
            return Task.FromResult(false);
        }

        Items[index] = entity;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken ct = default) =>
        Task.FromResult(Items.RemoveAll(e => e.Id == id) > 0);
}

public class FakeOrderRepository : InMemoryRepository<Order>, IOrderRepository
{
    public FakeOrderRepository(IEnumerable<Order>? seed = null) : base(seed) { }

    public Task<IReadOnlyList<Order>> GetByUserAsync(Guid userId, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Order>>(Items.Where(o => o.UserId == userId).ToList());

    public Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken ct = default) =>
        Task.FromResult(Items.FirstOrDefault(o => o.OrderNumber == orderNumber));

    public Task<int> CountAsync(CancellationToken ct = default) => Task.FromResult(Items.Count);
}

public class FakeProductRepository : InMemoryRepository<Product>, IProductRepository
{
    public FakeProductRepository(IEnumerable<Product>? seed = null) : base(seed) { }

    public Task<IReadOnlyList<Product>> GetByCategoryAsync(Guid categoryId, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Product>>(Items.Where(p => p.CategoryId == categoryId).ToList());

    public Task<IReadOnlyList<Product>> SearchAsync(string term, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Product>>(
            Items.Where(p => p.Name.Contains(term, StringComparison.OrdinalIgnoreCase)).ToList());
}

public class NullNotificationService : ChinaImportPlatform.Api.IServices.INotificationService
{
    public Task NotifyUserAsync(Guid userId, string title, string body, IDictionary<string, string>? data = null, CancellationToken ct = default) =>
        Task.CompletedTask;

    public Task BroadcastAsync(string? segment, string title, string body, IDictionary<string, string>? data = null, CancellationToken ct = default) =>
        Task.CompletedTask;
}
