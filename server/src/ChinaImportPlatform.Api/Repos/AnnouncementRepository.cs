using ChinaImportPlatform.Api.Common;
using ChinaImportPlatform.Api.IRepos;
using ChinaImportPlatform.Api.Models;

namespace ChinaImportPlatform.Api.Repos;

public class AnnouncementRepository : JsonRepository<Announcement>, IAnnouncementRepository
{
    public AnnouncementRepository(JsonStoreOptions options) : base(options, "announcements.json") { }

    public async Task<IReadOnlyList<Announcement>> GetPublishedAsync(CancellationToken ct = default) =>
        (await FindAsync(a => a.PublishedAt != null, ct))
            .OrderByDescending(a => a.PublishedAt)
            .ToList();
}
