using ChinaImportPlatform.Api.Data;
using ChinaImportPlatform.Api.IRepos;
using ChinaImportPlatform.Api.Models;

namespace ChinaImportPlatform.Api.Repos.Ef;

public class EfTripRepository : EfRepository<Trip>, ITripRepository
{
    public EfTripRepository(AppDbContext context) : base(context) { }
}
