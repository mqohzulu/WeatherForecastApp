using ChinaImportPlatform.Api.Models;

namespace ChinaImportPlatform.Api.IRepos;

public interface IOrderRepository : IRepository<Order>
{
    Task<IReadOnlyList<Order>> GetByUserAsync(Guid userId, CancellationToken ct = default);

    Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken ct = default);

    /// <summary>Returns the count of orders so a new human-readable order number can be generated.</summary>
    Task<int> CountAsync(CancellationToken ct = default);
}
