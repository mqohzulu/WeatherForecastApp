using ChinaImportPlatform.Api.Models;

namespace ChinaImportPlatform.Api.IRepos;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken ct = default);
}
