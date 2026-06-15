using ChinaImportPlatform.Api.Common;
using ChinaImportPlatform.Api.IRepos;
using ChinaImportPlatform.Api.Models;

namespace ChinaImportPlatform.Api.Repos;

public class DeviceTokenRepository : JsonRepository<DeviceToken>, IDeviceTokenRepository
{
    public DeviceTokenRepository(JsonStoreOptions options) : base(options, "device-tokens.json") { }

    public async Task<IReadOnlyList<DeviceToken>> GetByUserAsync(Guid userId, CancellationToken ct = default) =>
        await FindAsync(d => d.UserId == userId, ct);
}
