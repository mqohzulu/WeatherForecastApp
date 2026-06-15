using ChinaImportPlatform.Api.Data;
using ChinaImportPlatform.Api.IRepos;
using ChinaImportPlatform.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ChinaImportPlatform.Api.Repos.Ef;

public class EfConversationRepository : EfRepository<Conversation>, IConversationRepository
{
    public EfConversationRepository(AppDbContext context) : base(context) { }

    public async Task<Conversation?> GetByCustomerAsync(Guid customerId, CancellationToken ct = default) =>
        await Set.AsNoTracking()
            .FirstOrDefaultAsync(c => c.CustomerId == customerId, ct);
}
