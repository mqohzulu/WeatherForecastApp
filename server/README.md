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

## Authentication & authorization
- **Real JWTs.** `POST /api/v1/auth/verify-otp` returns a signed JWT access token and a
  refresh token. Send the access token as `Authorization: Bearer <token>` on every call.
- **Refresh-token rotation** (`POST /api/v1/auth/refresh`): the presented refresh token is
  revoked and a new pair is issued. Only SHA-256 hashes are stored (`refresh-tokens.json`).
- **Closed user base.** A fallback authorization policy requires authentication on every
  endpoint except `/health` and the three public auth endpoints. Seller-only management
  endpoints (catalogue writes, order queue, status/bulk/pricing, consolidated demand,
  broadcasts, payment requests, the inbox) require the `Seller` role.
- **OTP delivery** goes through `ISmsSender`. In Development the OTP is logged *and*
  returned as `devOtp` for testing; outside Development it is only sent by SMS.
- Configure `Jwt:SigningKey` (≥ 32 chars) — from AWS Secrets Manager in real environments.
  A development fallback key is used if none is set, so the API still runs out of the box.
- In Swagger UI, click **Authorize** and paste the access token to call secured endpoints.

## Notes on the scaffold
- **Notifications** are logged by `LoggingNotificationService`; production fans out to
  FCM/APNs behind the same `INotificationService` interface. Register device push tokens
  via `POST /api/v1/devices`.
