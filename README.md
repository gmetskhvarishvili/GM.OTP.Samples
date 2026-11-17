<p align="center">
  <img src="icon.png" alt="GM.OTP Samples" width="140" height="140" />
</p>

# GM.OTP Samples

[![CI](https://github.com/gmetskhvarishvili/GM.OTP.Samples/actions/workflows/ci.yml/badge.svg)](https://github.com/gmetskhvarishvili/GM.OTP.Samples/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

A runnable, multi-service demo of **[GM.OTP](https://www.nuget.org/packages/GM.OTP)** — issuing and
verifying one-time codes over an HTTP API, and emitting the generated code as an integration event
through **[GM.Messaging](https://www.nuget.org/packages/GM.Messaging)** (outbox → RabbitMQ → consumer)
for a downstream notifier to deliver. Built alongside
[GM.API](https://www.nuget.org/packages/GM.API), [GM.Mediator](https://www.nuget.org/packages/GM.Mediator)
and [GM.EntityFramework](https://www.nuget.org/packages/GM.EntityFramework). Targets `net10.0`.

## Projects

```
GM.OTP.Samples/
├── GM.OTP.Sample.API             # Web API — generate/verify OTP endpoints
├── GM.OTP.Sample.Application     # CQRS commands + validators (GenerateOtp / VerifyOtp)
├── GM.OTP.Sample.Domain          # Integration events, the OtpChallenge aggregate, inbox/outbox
├── GM.OTP.Sample.Infrastructure  # OTP service wiring (code generator / hasher / clock)
├── GM.OTP.Sample.Persistence     # EF Core persistence (PostgreSQL)
├── GM.OTP.Sample.Common          # Shared resources
├── GM.OTP.Sample.Producer.Worker # Relays the outbox to RabbitMQ
├── GM.OTP.Sample.Consumer.Worker # Consumes inbound OTP-request events
├── GM.OTP.Sample.Worker          # GM.Messaging inbox worker: ingested events → GenerateOtp
└── tests/
    └── GM.OTP.Sample.Tests       # command-validator, event, and aggregate unit tests
```

Only the published **GM.*** packages are referenced — no project references into the library repos.

## What it demonstrates

- **OTP generation & verification** — `GenerateOtp` issues a salted, subject/destination-bound code
  via `GM.OTP`'s `OtpManager` and persists an `OtpChallenge`; `VerifyOtp` checks a submitted code.
- **Outbox → messaging** — the generated code is published as an `OtpGeneratedIntegrationEvent`
  through the GM.Messaging outbox for a downstream service to deliver. The sample's `OutboxMessage`
  derives from GM.Messaging 5.0's `GMOutboxMessage` and is built with `OutboxMessage.Create(evt)`; it is
  mapped by `OutboxMessageConfiguration : GMOutboxMessageConfiguration<OutboxMessage>`. `AddPersistence`
  registers the outbox with one call, `AddGMOutbox<OutboxMessage, ApplicationDbContext>(configuration, relay: ...)`
  (administration, erasure events, save interceptors, the key ring from configuration, and the relay or the
  wake-up). Only the producer worker relays the outbox: it calls `AddPersistence(configuration, runOutboxRelay: true)`,
  while the API and the background worker call `AddPersistence` without the flag and wake the relay when they
  save events (GM.Messaging leases each message, so several relays would also be safe). The `outbox_messages`
  and `inbox_messages` tables share a `messaging` schema; the `UpgradeGMMessagingTo2` and `UpgradeGMMessagingTo3` migrations add
  the 2.0 and 3.0 columns. GM.Messaging 4.0 needs no database change, but it declares each exchange
  with the alternate exchange `gm.unrouted`, so messages with no queue bound for them are kept instead of
  dropped. RabbitMQ won't redeclare an existing exchange with a new argument: on a broker that already
  has the sample's exchanges, delete them once (the services recreate them when they start). GM.Messaging
  5.0 needs no database change either.
- **Personal data in events** — OTP destinations and the message carrying the code are `[SensitiveData]`
  (since GM.Messaging 4.4): encrypted in the outbox, the inbox and on RabbitMQ, plain in handlers. The key ring is
  `Messaging:SensitiveData` (a sample key in appsettings; use a secret store in production), shared with
  the Identity and Notifications samples. `AddGMOutbox(configuration, ...)` and `AddGMInbox(configuration)`
  register it from that section.
- **Outbox admin** — the API maps GM.Messaging's admin endpoints in `Program.cs` with
  `app.MapGMMessagingAdmin(...)`, under `/admin/messaging/outbox`: backlog, dead letters, requeue and purge.
  The sample has no login, so they're open (`admin.AllowAnonymous = true`); a real service passes its own
  policy with `admin.AuthorizationPolicy = "your-policy"`.
- **Inbox → OTP** — the consumer worker (`AddGMInbox<InboxMessage, ApplicationDbContext>(configuration)`)
  stores each incoming request with `IngestAsync`, and the
  background worker runs GM.Messaging's inbox worker (`AddGMInboxWorker`) with one
  `IInboxHandler<TEvent>` per event (`Worker/InboxHandlers`), registered with `AddGMInboxHandler`.
  Each handler sends `GenerateOtpCommand`; the challenge, its `OtpGenerated` outbox row and the
  inbox row commit together. Failures retry with backoff and are dead-lettered after the last
  attempt (`Messaging:Inbox` settings).
- **CQRS + validation** — mediator commands with FluentValidation.

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- **RabbitMQ** and **PostgreSQL** — the [GM.Messaging repo's `docker-compose.yml`](https://github.com/gmetskhvarishvili/GM.Messaging)
  spins both up locally.

## Running

Start RabbitMQ + PostgreSQL, then the API and the workers:

```bash
dotnet run --project GM.OTP.Sample.API
dotnet run --project GM.OTP.Sample.Producer.Worker
dotnet run --project GM.OTP.Sample.Consumer.Worker
```

`POST` a generate request to the API; the code is persisted and published as an event.

## Testing

```bash
dotnet test
```

The suite covers the command validators, the integration events, and the `OtpChallenge` aggregate —
pure unit tests that need no RabbitMQ or database.

## License

MIT — see [LICENSE](LICENSE).
