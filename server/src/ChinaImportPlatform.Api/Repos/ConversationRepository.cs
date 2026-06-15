using ChinaImportPlatform.Api.Common;
using ChinaImportPlatform.Api.IRepos;
using ChinaImportPlatform.Api.Models;

namespace ChinaImportPlatform.Api.Repos;

public class ConversationRepository : JsonRepository<Conversation>, IConversationRepository
{
    public ConversationRepository(JsonStoreOptions options) : base(options, "conversations.json") { }

    public async Task<Conversation?> GetByCustomerAsync(Guid customerId, CancellationToken ct = default) =>
        (await FindAsync(c => c.CustomerId == customerId, ct))
            .FirstOrDefault();
}
