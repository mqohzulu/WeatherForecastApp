# China Import & Distribution Platform — Implementation Plan

> A private, branded ordering and distribution application that replaces an informal
> WhatsApp-group ordering process with a secure mobile app (customers) and a management
> dashboard (seller). Flutter front end + .NET 8 REST API.

This document is the engineering plan for the repository. It maps the user stories
and technical direction from the business specification onto a concrete monorepo
structure, an API design, a data model, and a delivery roadmap.

---

## 1. Repository layout (monorepo: `server` + `client`)

```
WeatherForecastApp/                 # repository root (kept as the existing repo name)
├── server/                         # .NET 8 REST API (ASP.NET Core Web API)
│   ├── ChinaImportPlatform.sln
│   ├── src/ChinaImportPlatform.Api/
│   │   ├── Controllers/            # HTTP endpoints (thin)
│   │   ├── IServices/              # service interfaces (business-logic contracts)
│   │   ├── Services/               # service implementations (business logic)
│   │   ├── IRepos/                 # repository interfaces (persistence contracts)
│   │   ├── Repos/                  # JSON-backed repository implementations
│   │   ├── Models/                 # domain entities (DB-ready)
│   │   ├── Dtos/                   # request/response contracts
│   │   ├── Enums/                  # OrderStatus, PaymentStatus, ...
│   │   ├── Common/                 # ApiResponse, paging, mapping, middleware
│   │   ├── Data/
│   │   │   ├── AppDbContext.cs      # EF Core context for the future PostgreSQL DB
│   │   │   └── SeedData/*.json      # test data the JSON repositories read
│   │   └── Program.cs              # composition root / DI registration
│   └── tests/ChinaImportPlatform.Api.Tests/
└── client/china_import_app/        # Flutter app (feature-first clean architecture)
```

The two halves are deployed independently (separate CI pipelines) but live in one
repository for traceability between API contracts and the client that consumes them.

---

## 2. API architecture (Controllers → Services/IServices → Repos/IRepos)

The API uses a pragmatic layered architecture, exactly as requested:

| Layer | Folder | Responsibility |
|------|--------|----------------|
| **Controllers** | `Controllers/` | HTTP concerns only: routing, model binding, status codes. No business logic. |
| **Service contracts** | `IServices/` | Interfaces describing each use case. |
| **Services** | `Services/` | Business logic, validation, orchestration, mapping to DTOs. |
| **Repository contracts** | `IRepos/` | Persistence interfaces (`IRepository<T>` + per-aggregate interfaces). |
| **Repositories** | `Repos/` | Data access. Currently JSON-file backed; EF Core later. |
| **Models** | `Models/` | Domain entities, DB-ready and mappable by EF Core. |
| **DTOs** | `Dtos/` | The shapes that cross the wire; entities never leak out. |

**Dependency direction:** Controllers depend on `IServices`; Services depend on
`IRepos`; nothing depends on a concrete repository. This is why the database can be
swapped without touching controllers or business logic.

### Target language / framework
- **.NET 8** (`net8.0`), **C# 10** (`<LangVersion>10.0</LangVersion>`).
- Nullable reference types and implicit usings enabled.
- Serilog for logging, FluentValidation available for request validation,
  Swashbuckle for Swagger/OpenAPI.

### Data layer — JSON now, PostgreSQL later
The brief is "I will connect the DB myself, but for now set up test data in JSON and
have a repository pull from it." This is implemented as:

- `Repos/JsonRepository<T>` — a thread-safe, file-backed generic repository that loads
  a seed JSON file into an in-memory cache and (optionally) writes mutations back.
- Concrete repositories (`OrderRepository`, `ProductRepository`, …) bind to one file
  each and add aggregate-specific queries.
- `Data/SeedData/*.json` — realistic, relationally-consistent test data.

**Switching to PostgreSQL** (when you connect your DB):
1. Set `ConnectionStrings:Postgres` (ideally via AWS Secrets Manager).
2. Uncomment `AddDbContext<AppDbContext>(…UseNpgsql…)` in `Program.cs`.
3. Add EF Core implementations of the `IRepos` interfaces backed by `AppDbContext`,
   and register them instead of the JSON repositories.
4. `dotnet ef migrations add InitialCreate && dotnet ef database update`.

No controller or service changes are required — only the DI registrations.

### REST resource groups (all under `/api/v1`)
| Group | Endpoints (representative) | Stories |
|-------|----------------------------|---------|
| Auth | `POST /auth/request-otp`, `/auth/verify-otp`, `/auth/refresh`, `GET/PUT /auth/profile/{id}` | US-C01–C03, US-X01 |
| Categories | `GET/POST /categories`, `GET/PUT /categories/{id}` | US-C05, US-S11 |
| Products | `GET /products?categoryId=&search=`, `GET/POST/PUT/DELETE /products/{id}` | US-C05–C07, US-S11 |
| Trips | `GET/POST /trips`, `PUT /trips/{id}/status` | US-S03 |
| Orders | `GET /orders`, `POST /orders`, `PUT /orders/{id}/status`, `POST /orders/bulk-status`, `PUT /orders/{id}/pricing`, `POST /orders/{id}/cancel` | US-C08–C12, US-S01, S04–S06 |
| Consolidated demand | `GET /consolidated-demand?tripId=&minStatus=&categoryId=` | US-S02 |
| Announcements | `GET/POST /announcements` | US-C16, US-S08 |
| Payment requests | `GET /payment-requests/by-order/{id}`, `POST /payment-requests`, `POST .../proof`, `POST .../approve` | US-C18, US-S10, S13 |
| Conversations | `GET /conversations`, `GET .../messages`, `POST .../messages` | US-C17, US-S09 |
| Health | `GET /health` | Ops / CI smoke test |

Every endpoint returns a consistent `ApiResponse<T>` envelope (`success`, `data`,
`message`, `errors`). Errors are translated centrally by `ExceptionHandlingMiddleware`.

---

## 3. Data model

Monetary values are stored in **ZAR cents (integer)** to avoid rounding errors and to
make a future payment-gateway integration straightforward.

| Aggregate | Owns | Notes |
|-----------|------|-------|
| `User` | – | Phone-number identity, `Customer`/`Seller` role, soft-delete for POPIA. |
| `Category` | – | Data-driven; hidden automatically when empty. |
| `Product` | `ProductVariant[]`, `ProductImage[]` | Variants carry attribute maps (size/colour/model) + price adjustment. |
| `Trip` | – | Sourcing window with a cut-off date. |
| `Order` | `OrderItem[]`, `OrderStatusHistoryEntry[]` | Status history is an immutable audit trail. |
| `Announcement` | – | Broadcast; published + optional segment. |
| `Conversation` | – | One per customer. |
| `Message` | – | Stored server-side so history survives device changes. |
| `PaymentRequest` | `Payment[]` | Separated from payments so a gateway integration is cheap. |
| `DeviceToken` | – | One active push token per user per platform. |

The **consolidated demand list** (US-S02) is the key aggregation: order lines grouped
by `(productId, variantId)` across all orders in a trip window at or beyond `Confirmed`,
rolled up to total quantity and distinct customer count, expandable to the contributing
customers. See `ConsolidatedDemandService`.

---

## 4. In-app communication design (target)
Three cooperating delivery paths (built progressively):
1. **Real-time (SignalR):** foreground chat, read receipts, live order-status changes.
2. **Push (FCM/APNs):** background delivery with deep links. The current
   `INotificationService` logs only; production fans out to FCM.
3. **Pull (REST):** on app open the client syncs anything it missed so no event is lost.

---

## 5. Delivery roadmap (phased)

| Phase | Scope | Stories |
|------|-------|---------|
| **1 – MVP (8–10 wks)** | OTP auth, catalogue, cart + ordering, order status flow + notifications, seller queue, consolidated demand, single + bulk status updates. | US-C01–C03, C05–C09, C12, C14–C15; US-S01–S02, S04–S07, S11; US-X01–X04, X06 |
| **2 – Communication (3–4 wks)** | In-app chat (SignalR), announcements/broadcasts, trip windows, custom orders + quoting. | US-C10–C11, C13, C16–C17; US-S03, S08–S09, S12 |
| **3 – Payments (3–4 wks)** | Payment requests, proof upload + approval, reminders, finance summary; then hosted gateway. | US-C18–C19; US-S10, S13–S14 |
| **4 – Insight (2–3 wks)** | Analytics dashboards, exports, account deletion / POPIA tooling. | US-C04; US-S15; US-X05 |

**What this repository ships today:** the Phase-1 API surface end-to-end against JSON
test data — real JWT authentication with refresh-token rotation and role-based
authorization (closed user base; seller-only management endpoints), OTP delivery via a
pluggable `ISmsSender`, device push-token registration, catalogue, ordering, order
status + bulk updates, consolidated demand, trips, and announcements — plus scaffolding
for Phase-2/3 (messaging, payments) and a DB-ready EF Core context. The Flutter client
provides the matching feature-first architecture and the core flows.

**Still outstanding** (next up): EF Core repository implementations so the database
drops in; FCM/APNs behind `INotificationService`; SignalR real-time chat; the
accept/decline-quote flow; ready-for-collection fields; FluentValidation on writes; and
the Phase-3/4 features (payment reminders, finance summary, analytics, POPIA tooling).

---

## 6. Infrastructure & security direction (AWS)
- **Hosting:** containerised API on AWS App Runner (or ECS Fargate) behind ALB + AWS WAF.
- **DB:** Amazon RDS for PostgreSQL (af-south-1), encrypted, automated backups + PITR.
- **Files:** S3 (private) behind CloudFront; uploads via pre-signed URLs.
- **Secrets:** AWS Secrets Manager + IAM roles (no credentials in code).
- **Security:** short-lived JWTs with refresh rotation, TLS 1.2+, rate limiting on auth
  and messaging endpoints, immutable audit trails (order history, message timestamps).

## 7. CI/CD
- `.github/workflows/api.yml` — build, test, package (Docker), DB migrate, deploy.
- `.github/workflows/android.yml` and `ios.yml` — Flutter analyze/test, signed builds,
  store distribution. (Templates included as a starting point.)

---

## 8. How the requested constraints were honoured
- **API structure = Controllers, Services, IServices, Repos, IRepos** — exact folders.
- **.NET 8 + C# 10** — `net8.0` target with `<LangVersion>10.0</LangVersion>`.
- **DB connected later; JSON test data + repo that reads it** — `JsonRepository<T>` +
  `Data/SeedData/*.json`; EF Core `AppDbContext` is present but inactive until you wire it.
- **server / client split in one repo** — top-level `server/` and `client/` folders.
- **Best-practice Flutter structure** — feature-first clean architecture (see
  `client/china_import_app/README.md`).
