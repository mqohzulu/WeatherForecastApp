namespace ChinaImportPlatform.Api.Models;

/// <summary>
/// A catalogue product. Variants and images are modelled as owned collections
/// (one aggregate) which keeps the JSON store and the future EF mapping simple.
/// </summary>
public class Product : BaseEntity
{
    public Guid CategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    /// <summary>Indicative price in ZAR cents to avoid rounding errors.</summary>
    public long IndicativePriceCents { get; set; }

    /// <summary>e.g. "next trip – approximately 6 weeks".</summary>
    public string? LeadTime { get; set; }

    public bool IsActive { get; set; } = true;

    public List<ProductVariant> Variants { get; set; } = new();

    public List<ProductImage> Images { get; set; } = new();
}

/// <summary>A purchasable variation of a product (size/colour/model).</summary>
public class ProductVariant
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Free-form attributes such as size, colour, model.</summary>
    public Dictionary<string, string> Attributes { get; set; } = new();

    /// <summary>Adjustment in ZAR cents applied on top of the product's indicative price.</summary>
    public long PriceAdjustmentCents { get; set; }

    public bool IsActive { get; set; } = true;
}

/// <summary>A product image stored in S3 and served via CloudFront.</summary>
public class ProductImage
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string S3Key { get; set; } = string.Empty;

    public string? Url { get; set; }

    public int SortOrder { get; set; }
}
