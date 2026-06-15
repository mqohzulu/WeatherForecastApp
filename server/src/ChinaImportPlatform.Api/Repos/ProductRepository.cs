using ChinaImportPlatform.Api.Common;
using ChinaImportPlatform.Api.IRepos;
using ChinaImportPlatform.Api.Models;

namespace ChinaImportPlatform.Api.Repos;

public class ProductRepository : JsonRepository<Product>, IProductRepository
{
    public ProductRepository(JsonStoreOptions options) : base(options, "products.json") { }

    public async Task<IReadOnlyList<Product>> GetByCategoryAsync(Guid categoryId, CancellationToken ct = default) =>
        await FindAsync(p => p.CategoryId == categoryId && p.IsActive, ct);

    public async Task<IReadOnlyList<Product>> SearchAsync(string term, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return await FindAsync(p => p.IsActive, ct);
        }

        var needle = term.Trim();
        return await FindAsync(
            p => p.IsActive &&
                 (p.Name.Contains(needle, StringComparison.OrdinalIgnoreCase) ||
                  p.Description.Contains(needle, StringComparison.OrdinalIgnoreCase)),
            ct);
    }
}
