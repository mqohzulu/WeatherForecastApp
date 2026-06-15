using ChinaImportPlatform.Api.Models;

namespace ChinaImportPlatform.Api.IRepos;

public interface IConversationRepository : IRepository<Conversation>
{
    Task<Conversation?> GetByCustomerAsync(Guid customerId, CancellationToken ct = default);
}
