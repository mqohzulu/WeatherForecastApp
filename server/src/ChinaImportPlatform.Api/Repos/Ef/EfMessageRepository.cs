using ChinaImportPlatform.Api.Data;
using ChinaImportPlatform.Api.IRepos;
using ChinaImportPlatform.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ChinaImportPlatform.Api.Repos.Ef;

public class EfMessageRepository : EfRepository<Message>, IMessageRepository
{
    public EfMessageRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Message>> GetByConversationAsync(Guid conversationId, CancellationToken ct = default) =>
        await Set.AsNoTracking()
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.SentAt)
            .ToListAsync(ct);
}
