using ChinaImportPlatform.Api.Data;
using ChinaImportPlatform.Api.IRepos;
using ChinaImportPlatform.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ChinaImportPlatform.Api.Repos.Ef;

public class EfOrderRepository : EfRepository<Order>, IOrderRepository
{
    public EfOrderRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Order>> GetByUserAsync(Guid userId, CancellationToken ct = default) =>
        await Set.AsNoTracking()
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);

    public async Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken ct = default) =>
        await Set.AsNoTracking()
            .FirstOrDefaultAsync(o => EF.Functions.ILike(o.OrderNumber, orderNumber), ct);

    public async Task<int> CountAsync(CancellationToken ct = default) =>
        await Set.CountAsync(ct);
}
