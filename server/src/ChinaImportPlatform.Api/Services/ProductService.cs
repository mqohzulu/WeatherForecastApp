using ChinaImportPlatform.Api.Common;
using ChinaImportPlatform.Api.Dtos;
using ChinaImportPlatform.Api.IRepos;
using ChinaImportPlatform.Api.IServices;
using ChinaImportPlatform.Api.Models;

namespace ChinaImportPlatform.Api.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _products;

    public ProductService(IProductRepository products)
    {
        _products = products;
    }

    public async Task<PagedResult<ProductDto>> GetAsync(ProductQuery query, CancellationToken ct = default)
    {
        IEnumerable<Models.Product> source;

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            source = await _products.SearchAsync(query.Search, ct);
        }
        else if (query.CategoryId.HasValue)
        {
            source = await _products.GetByCategoryAsync(query.CategoryId.Value, ct);
        }
        else
        {
            source = await _products.FindAsync(p => p.IsActive, ct);
        }

        if (query.CategoryId.HasValue)
        {
            source = source.Where(p => p.CategoryId == query.CategoryId.Value);
        }

        if (query.MinPriceCents.HasValue)
        {
            source = source.Where(p => p.IndicativePriceCents >= query.MinPriceCents.Value);
        }

        if (query.MaxPriceCents.HasValue)
        {
            source = source.Where(p => p.IndicativePriceCents <= query.MaxPriceCents.Value);
        }

        source = query.Sort switch
        {
            "price_asc" => source.OrderBy(p => p.IndicativePriceCents),
            "price_desc" => source.OrderByDescending(p => p.IndicativePriceCents),
            "newest" => source.OrderByDescending(p => p.CreatedAt),
            _ => source.OrderBy(p => p.Name)
        };

        var dtos = source.Select(p => p.ToDto()).ToList();
        return PagedResult<ProductDto>.Create(dtos, query.Page, query.PageSize);
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var product = await _products.GetByIdAsync(id, ct);
        return product?.ToDto();
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken ct = default)
    {
        var product = new Product
        {
            CategoryId = dto.CategoryId,
            Name = dto.Name,
            Description = dto.Description,
            IndicativePriceCents = dto.IndicativePriceCents,
            LeadTime = dto.LeadTime,
            IsActive = true,
            Variants = dto.Variants.Select(v => new ProductVariant
            {
                Attributes = v.Attributes,
                PriceAdjustmentCents = v.PriceAdjustmentCents,
                IsActive = v.IsActive
            }).ToList(),
            Images = dto.Images.Select(i => new ProductImage
            {
                S3Key = i.S3Key,
                Url = i.Url,
                SortOrder = i.SortOrder
            }).ToList()
        };

        await _products.AddAsync(product, ct);
        return product.ToDto();
    }

    public async Task<ProductDto> UpdateAsync(Guid id, UpdateProductDto dto, CancellationToken ct = default)
    {
        var product = await _products.GetByIdAsync(id, ct)
                      ?? throw new NotFoundException($"Product {id} not found.");

        product.CategoryId = dto.CategoryId;
        product.Name = dto.Name;
        product.Description = dto.Description;
        product.IndicativePriceCents = dto.IndicativePriceCents;
        product.LeadTime = dto.LeadTime;
        product.IsActive = dto.IsActive;
        product.Variants = dto.Variants.Select(v => new ProductVariant
        {
            Id = v.Id == Guid.Empty ? Guid.NewGuid() : v.Id,
            Attributes = v.Attributes,
            PriceAdjustmentCents = v.PriceAdjustmentCents,
            IsActive = v.IsActive
        }).ToList();
        product.Images = dto.Images.Select(i => new ProductImage
        {
            Id = i.Id == Guid.Empty ? Guid.NewGuid() : i.Id,
            S3Key = i.S3Key,
            Url = i.Url,
            SortOrder = i.SortOrder
        }).ToList();

        await _products.UpdateAsync(product, ct);
        return product.ToDto();
    }

    public async Task<bool> HideAsync(Guid id, CancellationToken ct = default)
    {
        var product = await _products.GetByIdAsync(id, ct);
        if (product is null)
        {
            return false;
        }

        product.IsActive = false;
        return await _products.UpdateAsync(product, ct);
    }
}
