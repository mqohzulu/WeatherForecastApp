using ChinaImportPlatform.Api.Data;
using ChinaImportPlatform.Api.IRepos;
using ChinaImportPlatform.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ChinaImportPlatform.Api.Repos.Ef;

public class EfUserRepository : EfRepository<User>, IUserRepository
{
    public EfUserRepository(AppDbContext context) : base(context) { }

    public async Task<User?> GetByPhoneAsync(string phoneNumber, CancellationToken ct = default) =>
        await Set.AsNoTracking()
            .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber && !u.IsDeleted, ct);
}
