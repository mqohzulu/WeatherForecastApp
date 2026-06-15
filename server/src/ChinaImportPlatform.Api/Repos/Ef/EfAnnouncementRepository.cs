using ChinaImportPlatform.Api.Data;
using ChinaImportPlatform.Api.IRepos;
using ChinaImportPlatform.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ChinaImportPlatform.Api.Repos.Ef;

public class EfAnnouncementRepository : EfRepository<Announcement>, IAnnouncementRepository
{
    public EfAnnouncementRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Announcement>> GetPublishedAsync(CancellationToken ct = default) =>
        await Set.AsNoTracking()
            .Where(a => a.PublishedAt != null)
            .OrderByDescending(a => a.PublishedAt)
            .ToListAsync(ct);
}
