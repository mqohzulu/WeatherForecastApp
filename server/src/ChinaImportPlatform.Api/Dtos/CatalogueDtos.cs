namespace ChinaImportPlatform.Api.Dtos;

/// <summary>Catalogue query parameters: category + keyword + price range + sort + paging (US-C05/C07).</summary>
public record ProductQuery
{
    public Guid? CategoryId { get; init; }
    public string? Search { get; init; }
    public long? MinPriceCents { get; init; }
    public long? MaxPriceCents { get; init; }

    /// <summary>One of: name, price_asc, price_desc, newest. Defaults to name.</summary>
    public string? Sort { get; init; }

    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public record CategoryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int SortOrder { get; init; }
    public string? ImageUrl { get; init; }
    public bool IsActive { get; init; }
    public int ProductCount { get; init; }
}

public record CreateCategoryDto
{
    public string Name { get; init; } = string.Empty;
    public int SortOrder { get; init; }
    public string? ImageUrl { get; init; }
}

public record UpdateCategoryDto
{
    public string Name { get; init; } = string.Empty;
    public int SortOrder { get; init; }
    public string? ImageUrl { get; init; }
    public bool IsActive { get; init; } = true;
}

public record ProductVariantDto
{
    public Guid Id { get; init; }
    public Dictionary<string, string> Attributes { get; init; } = new();
    public long PriceAdjustmentCents { get; init; }
    public bool IsActive { get; init; }
}

public record ProductImageDto
{
    public Guid Id { get; init; }
    public string S3Key { get; init; } = string.Empty;
    public string? Url { get; init; }
    public int SortOrder { get; init; }
}

public record ProductDto
{
    public Guid Id { get; init; }
    public Guid CategoryId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public long IndicativePriceCents { get; init; }
    public string? LeadTime { get; init; }
    public bool IsActive { get; init; }
    public IReadOnlyList<ProductVariantDto> Variants { get; init; } = Array.Empty<ProductVariantDto>();
    public IReadOnlyList<ProductImageDto> Images { get; init; } = Array.Empty<ProductImageDto>();
}

public record CreateProductDto
{
    public Guid CategoryId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public long IndicativePriceCents { get; init; }
    public string? LeadTime { get; init; }
    public List<ProductVariantDto> Variants { get; init; } = new();
    public List<ProductImageDto> Images { get; init; } = new();
}

public record UpdateProductDto
{
    public Guid CategoryId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public long IndicativePriceCents { get; init; }
    public string? LeadTime { get; init; }
    public bool IsActive { get; init; } = true;
    public List<ProductVariantDto> Variants { get; init; } = new();
    public List<ProductImageDto> Images { get; init; } = new();
}
