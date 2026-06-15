namespace ChinaImportPlatform.Api.Models;

/// <summary>A data-driven product category so the seller can add/rename without a release.</summary>
public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    public string? ImageUrl { get; set; }

    public bool IsActive { get; set; } = true;
}
