using ChinaImportPlatform.Api.Common;
using ChinaImportPlatform.Api.IRepos;
using ChinaImportPlatform.Api.Models;

namespace ChinaImportPlatform.Api.Repos;

public class UserRepository : JsonRepository<User>, IUserRepository
{
    public UserRepository(JsonStoreOptions options) : base(options, "users.json") { }

    public async Task<User?> GetByPhoneAsync(string phoneNumber, CancellationToken ct = default) =>
        (await FindAsync(u => u.PhoneNumber == phoneNumber && !u.IsDeleted, ct))
            .FirstOrDefault();
}
