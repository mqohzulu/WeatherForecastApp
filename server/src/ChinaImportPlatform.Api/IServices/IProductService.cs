using ChinaImportPlatform.Api.Common;
using ChinaImportPlatform.Api.Dtos;

namespace ChinaImportPlatform.Api.IServices;

public interface IProductService
{
    Task<PagedResult<ProductDto>> GetAsync(ProductQuery query, CancellationToken ct = default);

    Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken ct = default);

    Task<ProductDto> UpdateAsync(Guid id, UpdateProductDto dto, CancellationToken ct = default);

    /// <summary>Hides a product from the catalogue while preserving it on historical orders.</summary>
    Task<bool> HideAsync(Guid id, CancellationToken ct = default);
}
