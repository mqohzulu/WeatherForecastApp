using ChinaImportPlatform.Api.Data;
using ChinaImportPlatform.Api.IRepos;
using ChinaImportPlatform.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ChinaImportPlatform.Api.Repos.Ef;

public class EfProductRepository : EfRepository<Product>, IProductRepository
{
    public EfProductRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Product>> GetByCategoryAsync(Guid categoryId, CancellationToken ct = default) =>
        await Set.AsNoTracking()
            .Where(p => p.CategoryId == categoryId && p.IsActive)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Product>> SearchAsync(string term, CancellationToken ct = default)
    {
        var query = Set.AsNoTracking().Where(p => p.IsActive);

        if (!string.IsNullOrWhiteSpace(term))
        {
            // ILike is case-insensitive on PostgreSQL (Npgsql).
            var pattern = $"%{term.Trim()}%";
            query = query.Where(p =>
                EF.Functions.ILike(p.Name, pattern) ||
                EF.Functions.ILike(p.Description, pattern));
        }

        return await query.ToListAsync(ct);
    }
}
