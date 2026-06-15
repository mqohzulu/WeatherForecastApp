using ChinaImportPlatform.Api.Data;
using ChinaImportPlatform.Api.IRepos;
using ChinaImportPlatform.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ChinaImportPlatform.Api.Repos.Ef;

public class EfCategoryRepository : EfRepository<Category>, ICategoryRepository
{
    public EfCategoryRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Category>> GetActiveAsync(CancellationToken ct = default) =>
        await Set.AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.SortOrder)
            .ToListAsync(ct);
}
