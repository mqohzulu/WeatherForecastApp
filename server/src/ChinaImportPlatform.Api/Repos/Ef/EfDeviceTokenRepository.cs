using ChinaImportPlatform.Api.Data;
using ChinaImportPlatform.Api.IRepos;
using ChinaImportPlatform.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ChinaImportPlatform.Api.Repos.Ef;

public class EfDeviceTokenRepository : EfRepository<DeviceToken>, IDeviceTokenRepository
{
    public EfDeviceTokenRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<DeviceToken>> GetByUserAsync(Guid userId, CancellationToken ct = default) =>
        await Set.AsNoTracking()
            .Where(d => d.UserId == userId)
            .ToListAsync(ct);
}
