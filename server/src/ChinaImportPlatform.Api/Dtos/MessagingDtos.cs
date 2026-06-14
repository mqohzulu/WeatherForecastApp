namespace ChinaImportPlatform.Api.Dtos;

public record MessageDto
{
    public Guid Id { get; init; }
    public Guid ConversationId { get; init; }
    public Guid SenderId { get; init; }
    public Guid? OrderId { get; init; }
    public string Body { get; init; } = string.Empty;
    public string? AttachmentS3Key { get; init; }
    public DateTime SentAt { get; init; }
    public DateTime? DeliveredAt { get; init; }
    public DateTime? ReadAt { get; init; }
}

public record ConversationDto
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }
    public string? CustomerName { get; init; }
    public DateTime? LastMessageAt { get; init; }
    public int UnreadForSeller { get; init; }
    public int UnreadForCustomer { get; init; }
}

public record SendMessageDto
{
    public Guid ConversationId { get; init; }
    public Guid SenderId { get; init; }
    public Guid? OrderId { get; init; }
    public string Body { get; init; } = string.Empty;
    public string? AttachmentS3Key { get; init; }
}
