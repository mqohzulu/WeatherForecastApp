using ChinaImportPlatform.Api.Common;
using ChinaImportPlatform.Api.Dtos;
using ChinaImportPlatform.Api.Enums;
using ChinaImportPlatform.Api.IRepos;
using ChinaImportPlatform.Api.IServices;
using ChinaImportPlatform.Api.Models;

namespace ChinaImportPlatform.Api.Services;

public class MessagingService : IMessagingService
{
    private readonly IConversationRepository _conversations;
    private readonly IMessageRepository _messages;
    private readonly IUserRepository _users;
    private readonly INotificationService _notifications;

    public MessagingService(
        IConversationRepository conversations,
        IMessageRepository messages,
        IUserRepository users,
        INotificationService notifications)
    {
        _conversations = conversations;
        _messages = messages;
        _users = users;
        _notifications = notifications;
    }

    public async Task<IReadOnlyList<ConversationDto>> GetConversationsAsync(CancellationToken ct = default)
    {
        var conversations = await _conversations.GetAllAsync(ct);
        var users = (await _users.GetAllAsync(ct)).ToDictionary(u => u.Id);

        return conversations
            .OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt)
            .Select(c => c.ToDto(users.TryGetValue(c.CustomerId, out var u) ? u.FullName : null))
            .ToList();
    }

    public async Task<ConversationDto> GetOrCreateForCustomerAsync(Guid customerId, CancellationToken ct = default)
    {
        var conversation = await _conversations.GetByCustomerAsync(customerId, ct);
        if (conversation is null)
        {
            conversation = new Conversation { CustomerId = customerId };
            await _conversations.AddAsync(conversation, ct);
        }

        var user = await _users.GetByIdAsync(customerId, ct);
        return conversation.ToDto(user?.FullName);
    }

    public async Task<IReadOnlyList<MessageDto>> GetMessagesAsync(Guid conversationId, CancellationToken ct = default) =>
        (await _messages.GetByConversationAsync(conversationId, ct)).Select(m => m.ToDto()).ToList();

    public async Task<MessageDto> SendAsync(SendMessageDto dto, CancellationToken ct = default)
    {
        var conversation = await _conversations.GetByIdAsync(dto.ConversationId, ct)
                           ?? throw new NotFoundException($"Conversation {dto.ConversationId} not found.");

        var message = new Message
        {
            ConversationId = dto.ConversationId,
            SenderId = dto.SenderId,
            OrderId = dto.OrderId,
            Body = dto.Body,
            AttachmentS3Key = dto.AttachmentS3Key,
            SentAt = DateTime.UtcNow
        };
        await _messages.AddAsync(message, ct);

        conversation.LastMessageAt = message.SentAt;

        // Increment the unread counter for whichever party did not send the message.
        var sender = await _users.GetByIdAsync(dto.SenderId, ct);
        if (sender?.Role == UserRole.Seller)
        {
            conversation.UnreadForCustomer++;
            await _notifications.NotifyUserAsync(conversation.CustomerId, "New message", dto.Body, null, ct);
        }
        else
        {
            conversation.UnreadForSeller++;
        }

        await _conversations.UpdateAsync(conversation, ct);
        return message.ToDto();
    }
}
