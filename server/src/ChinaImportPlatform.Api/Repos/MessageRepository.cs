using ChinaImportPlatform.Api.Common;
using ChinaImportPlatform.Api.IRepos;
using ChinaImportPlatform.Api.Models;

namespace ChinaImportPlatform.Api.Repos;

public class MessageRepository : JsonRepository<Message>, IMessageRepository
{
    public MessageRepository(JsonStoreOptions options) : base(options, "messages.json") { }

    public async Task<IReadOnlyList<Message>> GetByConversationAsync(Guid conversationId, CancellationToken ct = default) =>
        (await FindAsync(m => m.ConversationId == conversationId, ct))
            .OrderBy(m => m.SentAt)
            .ToList();
}
