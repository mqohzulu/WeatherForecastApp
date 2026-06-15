using ChinaImportPlatform.Api.Models;

namespace ChinaImportPlatform.Api.IRepos;

public interface IDeviceTokenRepository : IRepository<DeviceToken>
{
    Task<IReadOnlyList<DeviceToken>> GetByUserAsync(Guid userId, CancellationToken ct = default);
}
