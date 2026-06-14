using ChinaImportPlatform.Api.Common;
using ChinaImportPlatform.Api.Dtos;
using ChinaImportPlatform.Api.IRepos;
using ChinaImportPlatform.Api.IServices;
using ChinaImportPlatform.Api.Models;

namespace ChinaImportPlatform.Api.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categories;
    private readonly IProductRepository _products;

    public CategoryService(ICategoryRepository categories, IProductRepository products)
    {
        _categories = categories;
        _products = products;
    }

    public async Task<IReadOnlyList<CategoryDto>> GetActiveAsync(CancellationToken ct = default)
    {
        var categories = await _categories.GetActiveAsync(ct);
        var products = await _products.GetAllAsync(ct);

        // Categories with no active products are hidden from customers (US-C05).
        return categories
            .Select(c => c.ToDto(products.Count(p => p.CategoryId == c.Id && p.IsActive)))
            .Where(c => c.ProductCount > 0)
            .ToList();
    }

    public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken ct = default)
    {
        var categories = await _categories.GetAllAsync(ct);
        var products = await _products.GetAllAsync(ct);

        return categories
            .OrderBy(c => c.SortOrder)
            .Select(c => c.ToDto(products.Count(p => p.CategoryId == c.Id && p.IsActive)))
            .ToList();
    }

    public async Task<CategoryDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var category = await _categories.GetByIdAsync(id, ct);
        if (category is null)
        {
            return null;
        }

        var count = (await _products.GetByCategoryAsync(id, ct)).Count;
        return category.ToDto(count);
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto, CancellationToken ct = default)
    {
        var category = new Category
        {
            Name = dto.Name,
            SortOrder = dto.SortOrder,
            ImageUrl = dto.ImageUrl,
            IsActive = true
        };

        await _categories.AddAsync(category, ct);
        return category.ToDto();
    }

    public async Task<CategoryDto> UpdateAsync(Guid id, UpdateCategoryDto dto, CancellationToken ct = default)
    {
        var category = await _categories.GetByIdAsync(id, ct)
                       ?? throw new NotFoundException($"Category {id} not found.");

        category.Name = dto.Name;
        category.SortOrder = dto.SortOrder;
        category.ImageUrl = dto.ImageUrl;
        category.IsActive = dto.IsActive;

        await _categories.UpdateAsync(category, ct);
        return category.ToDto();
    }
}
