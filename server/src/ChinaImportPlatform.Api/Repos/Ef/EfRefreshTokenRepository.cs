using ChinaImportPlatform.Api.Data;
using ChinaImportPlatform.Api.IRepos;
using ChinaImportPlatform.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ChinaImportPlatform.Api.Repos.Ef;

public class EfRefreshTokenRepository : EfRepository<RefreshToken>, IRefreshTokenRepository
{
    public EfRefreshTokenRepository(AppDbContext context) : base(context) { }

    public async Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken ct = default) =>
        await Set.AsNoTracking()
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, ct);
}
