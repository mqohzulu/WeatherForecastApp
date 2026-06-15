using ChinaImportPlatform.Api.Models;

namespace ChinaImportPlatform.Api.IRepos;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByPhoneAsync(string phoneNumber, CancellationToken ct = default);
}
