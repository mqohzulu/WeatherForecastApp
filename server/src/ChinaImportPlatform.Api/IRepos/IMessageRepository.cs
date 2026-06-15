using ChinaImportPlatform.Api.Models;

namespace ChinaImportPlatform.Api.IRepos;

public interface IMessageRepository : IRepository<Message>
{
    Task<IReadOnlyList<Message>> GetByConversationAsync(Guid conversationId, CancellationToken ct = default);
}
