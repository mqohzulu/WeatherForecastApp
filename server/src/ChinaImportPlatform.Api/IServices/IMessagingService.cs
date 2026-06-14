using ChinaImportPlatform.Api.Dtos;

namespace ChinaImportPlatform.Api.IServices;

public interface IMessagingService
{
    Task<IReadOnlyList<ConversationDto>> GetConversationsAsync(CancellationToken ct = default);

    Task<ConversationDto> GetOrCreateForCustomerAsync(Guid customerId, CancellationToken ct = default);

    Task<IReadOnlyList<MessageDto>> GetMessagesAsync(Guid conversationId, CancellationToken ct = default);

    Task<MessageDto> SendAsync(SendMessageDto dto, CancellationToken ct = default);
}
