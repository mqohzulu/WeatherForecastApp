namespace ChinaImportPlatform.Api.Models;

/// <summary>
/// Base type for all persisted aggregates. Uses a GUID primary key so that
/// records can be created on the client (offline) and on the server without
/// collisions, matching the offline-submission requirement in the plan.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
