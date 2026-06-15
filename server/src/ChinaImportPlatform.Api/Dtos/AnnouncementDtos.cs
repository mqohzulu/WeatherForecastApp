namespace ChinaImportPlatform.Api.Dtos;

public record AnnouncementDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Body { get; init; } = string.Empty;
    public string? ImageS3Key { get; init; }
    public DateTime? CutoffDate { get; init; }
    public string? Segment { get; init; }
    public DateTime? PublishedAt { get; init; }
}

public record CreateAnnouncementDto
{
    public string Title { get; init; } = string.Empty;
    public string Body { get; init; } = string.Empty;
    public string? ImageS3Key { get; init; }
    public DateTime? CutoffDate { get; init; }
    public string? Segment { get; init; }

    /// <summary>When true the announcement is published immediately (visible + pushed).</summary>
    public bool PublishNow { get; init; } = true;
}
