using ChinaImportPlatform.Api.Common;
using ChinaImportPlatform.Api.IRepos;
using ChinaImportPlatform.Api.Models;

namespace ChinaImportPlatform.Api.Repos;

public class OrderRepository : JsonRepository<Order>, IOrderRepository
{
    public OrderRepository(JsonStoreOptions options) : base(options, "orders.json") { }

    public async Task<IReadOnlyList<Order>> GetByUserAsync(Guid userId, CancellationToken ct = default) =>
        (await FindAsync(o => o.UserId == userId, ct))
            .OrderByDescending(o => o.CreatedAt)
            .ToList();

    public async Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken ct = default) =>
        (await FindAsync(o => string.Equals(o.OrderNumber, orderNumber, StringComparison.OrdinalIgnoreCase), ct))
            .FirstOrDefault();

    public async Task<int> CountAsync(CancellationToken ct = default) =>
        (await GetAllAsync(ct)).Count;
}
