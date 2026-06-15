# China Import & Distribution Platform

A private, branded ordering and distribution application that replaces an informal
WhatsApp-group ordering process with a secure mobile app for customers and a management
dashboard for the seller. The seller imports goods from China, applies a markup, and
distributes them to customers in South Africa.

This is a **monorepo** with two independently deployable halves:

| Folder | What it is | Stack |
|--------|------------|-------|
| [`server/`](server/) | REST API | .NET 8 (C# 10), ASP.NET Core Web API, JSON-backed repositories (PostgreSQL-ready via EF Core) |
| [`client/china_import_app/`](client/china_import_app/) | Mobile app (Android + iOS) | Flutter, feature-first clean architecture, Riverpod |

📋 The full engineering plan is in [`docs/IMPLEMENTATION_PLAN.md`](docs/IMPLEMENTATION_PLAN.md).

---

## Quick start

### API (server)
```bash
cd server
dotnet restore
dotnet run --project src/ChinaImportPlatform.Api
# Swagger UI: http://localhost:5000/swagger
# Health:     http://localhost:5000/health
```
The API boots against the JSON test data in
`server/src/ChinaImportPlatform.Api/Data/SeedData/` — **no database is required to run it.**
See [`server/README.md`](server/README.md) for how to connect your own PostgreSQL database.

### App (client)
```bash
cd client/china_import_app
flutter pub get
dart run build_runner build --delete-conflicting-outputs
flutter run --dart-define=ENV=dev
```
See [`client/china_import_app/README.md`](client/china_import_app/README.md) for the
architecture and tooling.

---

## API design at a glance

Layered exactly as **Controllers → Services / IServices → Repos / IRepos**, with
DTOs, domain models, and enums in their own folders. The data layer is JSON-backed today
and swaps to EF Core + PostgreSQL by changing only the dependency-injection registrations
in `Program.cs` — controllers and business logic stay untouched.

Key REST groups (all under `/api/v1`): `auth`, `categories`, `products`, `trips`,
`orders` (+ status / bulk-status / pricing), `consolidated-demand`, `announcements`,
`payment-requests`, `conversations`.

## License / ownership
Compiled for Energy and Combustion Services Technology Solutions Development (Pty) Ltd.
