namespace ChinaImportPlatform.Api.Models;

/// <summary>A one-to-many broadcast from the seller, shown in the feed and pushed.</summary>
public class Announcement : BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;

    public string? ImageS3Key { get; set; }

    public DateTime? CutoffDate { get; set; }

    /// <summary>Audience segment, e.g. "all" or a category name. Null means all customers.</summary>
    public string? Segment { get; set; }

    public DateTime? PublishedAt { get; set; }
}
