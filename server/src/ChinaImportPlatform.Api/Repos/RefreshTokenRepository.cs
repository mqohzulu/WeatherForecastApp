using ChinaImportPlatform.Api.Common;
using ChinaImportPlatform.Api.IRepos;
using ChinaImportPlatform.Api.Models;

namespace ChinaImportPlatform.Api.Repos;

public class RefreshTokenRepository : JsonRepository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(JsonStoreOptions options) : base(options, "refresh-tokens.json") { }

    public async Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken ct = default) =>
        (await FindAsync(t => t.TokenHash == tokenHash, ct)).FirstOrDefault();
}
