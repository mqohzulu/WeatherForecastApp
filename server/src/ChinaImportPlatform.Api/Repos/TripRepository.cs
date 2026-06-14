using ChinaImportPlatform.Api.Common;
using ChinaImportPlatform.Api.IRepos;
using ChinaImportPlatform.Api.Models;

namespace ChinaImportPlatform.Api.Repos;

public class TripRepository : JsonRepository<Trip>, ITripRepository
{
    public TripRepository(JsonStoreOptions options) : base(options, "trips.json") { }
}
