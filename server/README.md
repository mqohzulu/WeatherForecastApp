# China Import & Distribution Platform — API

ASP.NET Core Web API on **.NET 8** using **C# 10**, laid out as
**Controllers → Services / IServices → Repos / IRepos**. It runs against JSON test data
out of the box and is ready to switch to PostgreSQL when you connect your database.

## Run it
```bash
cd server
dotnet restore
dotnet run --project src/ChinaImportPlatform.Api
```
- Swagger UI: `http://localhost:5000/swagger`
- Health check: `http://localhost:5000/health`
- Sample requests: `src/ChinaImportPlatform.Api/ChinaImportPlatform.Api.http`

## Run the tests
```bash
cd server
dotnet test
```

## Project structure
```
src/ChinaImportPlatform.Api/
├── Controllers/    # HTTP endpoints (thin; no business logic)
├── IServices/      # business-logic contracts
├── Services/       # business logic + DTO mapping
├── IRepos/         # persistence contracts (IRepository<T> + per-aggregate)
├── Repos/          # JSON-backed repositories (JsonRepository<T> base)
├── Models/         # domain entities (EF Core mappable)
├── Dtos/           # request/response shapes
├── Enums/          # OrderStatus, PaymentStatus, ...
├── Common/         # ApiResponse, paging, mapping, exception middleware
├── Data/
│   ├── AppDbContext.cs    # EF Core context for the future PostgreSQL DB
│   └── SeedData/*.json    # the test data the repositories read
└── Program.cs      # DI composition root
```

## Where the data comes from
The repositories read the JSON files in `Data/SeedData/`. `JsonRepository<T>` loads each
file into a thread-safe in-memory cache and writes mutations back to disk (configurable
via `JsonStore:PersistChanges`). The seed data is relationally consistent — e.g. the
consolidated-demand endpoint returns "14 angle grinders across 2 customers" from the
seeded orders.

## Connecting your PostgreSQL database
1. Put the connection string in `ConnectionStrings:Postgres` (use AWS Secrets Manager in
   real environments).
2. In `Program.cs`, uncomment the `AddDbContext<AppDbContext>(… UseNpgsql …)` block.
3. Add EF Core implementations of the `IRepos` interfaces backed by `AppDbContext` and
   register them in place of the JSON repositories.
4. Create and apply the schema:
   ```bash
   dotnet ef migrations add InitialCreate --project src/ChinaImportPlatform.Api
   dotnet ef database update --project src/ChinaImportPlatform.Api
   ```
Because controllers and services depend only on the interfaces, **no other code changes.**

## Notes on the scaffold
- **Auth** is a development stub: OTPs are generated in memory and returned in the
  response (`devOtp`); tokens are opaque placeholders. Production will send OTPs by SMS
  and issue signed JWTs with refresh-token rotation.
- **Notifications** are logged by `LoggingNotificationService`; production fans out to
  FCM/APNs behind the same `INotificationService` interface.
