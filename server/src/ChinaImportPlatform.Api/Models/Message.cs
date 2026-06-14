namespace ChinaImportPlatform.Api.Models;

/// <summary>A chat message. Stored server-side so history survives device changes.</summary>
public class Message : BaseEntity
{
    public Guid ConversationId { get; set; }

    public Guid SenderId { get; set; }

    /// <summary>Optionally attaches the message to a specific order for context.</summary>
    public Guid? OrderId { get; set; }

    public string Body { get; set; } = string.Empty;

    public string? AttachmentS3Key { get; set; }

    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    public DateTime? DeliveredAt { get; set; }

    public DateTime? ReadAt { get; set; }
}
