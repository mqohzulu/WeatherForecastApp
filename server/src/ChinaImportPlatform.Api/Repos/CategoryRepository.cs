using ChinaImportPlatform.Api.Common;
using ChinaImportPlatform.Api.IRepos;
using ChinaImportPlatform.Api.Models;

namespace ChinaImportPlatform.Api.Repos;

public class CategoryRepository : JsonRepository<Category>, ICategoryRepository
{
    public CategoryRepository(JsonStoreOptions options) : base(options, "categories.json") { }

    public async Task<IReadOnlyList<Category>> GetActiveAsync(CancellationToken ct = default) =>
        (await FindAsync(c => c.IsActive, ct))
            .OrderBy(c => c.SortOrder)
            .ToList();
}
