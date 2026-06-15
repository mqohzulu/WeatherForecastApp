using ChinaImportPlatform.Api.Models;

namespace ChinaImportPlatform.Api.IRepos;

public interface IProductRepository : IRepository<Product>
{
    Task<IReadOnlyList<Product>> GetByCategoryAsync(Guid categoryId, CancellationToken ct = default);

    Task<IReadOnlyList<Product>> SearchAsync(string term, CancellationToken ct = default);
}
