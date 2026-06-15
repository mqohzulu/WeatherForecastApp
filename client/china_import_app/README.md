# China Import & Distribution Platform — Flutter Client

A private, branded ordering application that replaces a WhatsApp-based ordering
process. It talks to a **.NET 8 REST API** (`/api/v1/...`) and serves two roles:

- **Customer** — browse the catalogue, build a cart, place (and track) orders,
  read announcements, chat with the seller, and submit proof of payment.
- **Seller / Admin** — work the order queue, view consolidated demand, update
  order statuses, and broadcast announcements.

---

## Architecture

The project follows a **feature-first Clean Architecture** with three layers
per feature:

```
presentation  ->  domain  ->  data
  (UI, Riverpod)   (entities,    (models, datasources,
                    repos[abstract], repository impls)
                    usecases)
```

- **domain** is pure Dart: entities, abstract repository contracts, and use
  cases. It depends on nothing in `data` or `presentation`.
- **data** implements the domain repositories, talking to remote datasources
  (Dio) and mapping JSON models (`*_model.dart`, with `toEntity()`) to entities.
- **presentation** holds Riverpod providers and Flutter widgets/screens.

Cross-cutting concerns live in `core/` and are shared via Riverpod providers.

### Error handling

Repositories return `Result<T>` (a `typedef` over fpdart's
`Either<Failure, T>`). Datasource/Dio errors are converted to typed
`AppException`s (`core/error/exceptions.dart`) and then to user-facing
`Failure`s (`core/error/failure.dart`) by the `RepositoryHelper.guard()` mixin.
The presentation layer only ever sees `Failure`, never networking details.

### Networking

`core/network/dio_client.dart` builds the `Dio` instance with:

- **`AuthInterceptor`** — attaches the `Bearer` access token and, on a `401`,
  transparently calls `POST /auth/refresh` (using a separate bare `Dio` to
  avoid recursion), persists the new tokens, and retries the original request.
  If refresh fails it clears tokens and raises the session-expired signal.
- **`LoggingInterceptor`** — verbose request/response logging (dev/staging only).

### Folder layout

```
lib/
  main.dart                 # entrypoint -> bootstrap()
  bootstrap.dart            # config init, error handling, ProviderScope, runApp
  app/
    app.dart                # MaterialApp.router
    router/                 # GoRouter config + route constants
    theme/                  # colours, typography, Material 3 themes
  core/
    config/                 # env-based AppConfig + Flavor
    constants/              # app constants + API endpoint paths
    network/                # Dio client, interceptors, Result, exceptions
    storage/                # secure storage + token storage
    error/                  # Failure + AppException hierarchies
    utils/                  # validators, formatters (ZAR/date), logger
    di/                     # core Riverpod providers (dio, storage, ...)
    notifications/          # push notification service (stubbed)
    widgets/                # shared reusable widgets
  features/
    auth/ catalogue/ cart/ orders/ announcements/
    messaging/ payments/ seller/
        data/ domain/ presentation/
  shared/
    models/                 # cross-feature models (PaginatedResponse, ...)
test/                       # unit/widget tests
```

---

## State management — Riverpod

We use **Riverpod 2** (`flutter_riverpod`) because it gives us:

- compile-safe dependency injection (`Provider`, `.family`, overrides for tests),
- ergonomic async state via `FutureProvider` + `AsyncValue.when(...)`,
- testability — every provider can be overridden in a `ProviderContainer`.

Patterns used:

- `Provider` for wiring datasources / repositories / use cases.
- `StateNotifierProvider` for screens with imperative flows (e.g. `authProvider`,
  `cartProvider`).
- `FutureProvider` / `FutureProvider.family` for read models (catalogue, orders,
  announcements, demand).

`riverpod_generator` + `riverpod_annotation` are included so new providers can
optionally use the codegen (`@riverpod`) style.

---

## Configuration & flavors

The active environment is chosen at build time via `--dart-define=ENV=...`:

| ENV       | API base URL                                   |
|-----------|------------------------------------------------|
| `dev`     | `http://10.0.2.2:5000` (Android emulator host) |
| `staging` | `https://staging-api.china-import.example.co.za` |
| `prod`    | `https://api.china-import.example.co.za` (placeholder) |

The version prefix `/api/v1` is appended automatically (`AppConfig.apiUrl`).
Update the staging/prod URLs in `lib/core/config/app_config.dart`.

---

## Getting started

> **Prerequisites:** Flutter SDK (Dart `>=3.4.0 <4.0.0`).

This repository contains the `lib/`, `test/`, and project metadata. The native
platform folders (`android/`, `ios/`, etc.) are intentionally not committed —
generate them once on first checkout:

```bash
# from client/china_import_app/
flutter create . --project-name china_import_app --platforms=android,ios

# 1. install dependencies
flutter pub get

# 2. generate freezed / json_serializable / riverpod code
dart run build_runner build --delete-conflicting-outputs

# 3. run against the dev API
flutter run --dart-define=ENV=dev
```

### Building

```bash
flutter build apk     --dart-define=ENV=prod
flutter build appbundle --dart-define=ENV=prod
flutter build ipa     --dart-define=ENV=prod
```

### Code generation

Models annotated with `@JsonSerializable()` (and any future `@freezed` /
`@riverpod`) rely on generated `*.g.dart` / `*.freezed.dart` files. These are
**git-ignored** and must be (re)generated after pulling or changing a model:

```bash
dart run build_runner watch --delete-conflicting-outputs   # during development
```

---

## Testing

```bash
flutter test
```

Tests use `flutter_test` and `mocktail`. Examples included:

- `test/validators_test.dart` — SA mobile number / OTP validation.
- `test/auth_provider_test.dart` — `AuthNotifier` flow with a mocked
  `AuthRepository`, overridden via `ProviderContainer`.

Because every dependency is a Riverpod provider, tests override the repository
provider with a mock and drive the notifier directly — no network required.

---

## Domain notes

- **Auth:** phone-number + OTP, JWT access + refresh tokens stored in
  `flutter_secure_storage`. Tokens auto-refresh on `401`.
- **Order status timeline:** `placed → confirmed → beingSourced → shipped →
  arrivedInSa → readyForCollection → completed`, plus terminal
  `cancelled` / `rejected`. See `features/orders/.../order_status.dart`.
- **Currency:** all prices are ZAR, formatted via `intl` `NumberFormat`
  (`Formatters.zar`).
- **Payments:** manual phase first — banking details + proof-of-payment upload
  (`image_picker`).
- **Messaging:** request/response now; SignalR real-time is a planned follow-up.
- **Push notifications:** `core/notifications/push_notification_service.dart`
  is a stub (`NoopPushNotificationService`) pending FCM/APNs integration.

---

## Conventions

- `analysis_options.yaml` enables `flutter_lints` plus stricter rules
  (single quotes, trailing commas, `prefer_const`, return types, no `print`).
- Imports within a feature/core use relative paths; generated files are excluded
  from analysis.
