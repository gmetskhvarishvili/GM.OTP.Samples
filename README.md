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
  through the GM.Messaging outbox for a downstream service to deliver. Only the producer worker
  relays the outbox: it calls `AddPersistence(configuration, runOutboxRelay: true)`, while the API
  and the background worker call `AddPersistence` without the flag, so publishing stays in one host
  (GM.Messaging 2.0 leases each message, so several relays would also be safe). The `outbox_messages`
  and `inbox_messages` tables share a `messaging` schema; the `UpgradeGMMessagingTo2` and `UpgradeGMMessagingTo3` migrations add
  the 2.0 and 3.0 columns. GM.Messaging 4.0 needs no database change, but it declares each exchange
  with the alternate exchange `gm.unrouted`, so messages with no queue bound for them are kept instead of
  dropped. RabbitMQ won't redeclare an existing exchange with a new argument: on a broker that already
  has the sample's exchanges, delete them once (the services recreate them when they start).
- **Outbox admin** — the API maps GM.Messaging's admin endpoints (`MapGMMessagingAdmin`) under
  `/admin/messaging/outbox`: backlog, dead letters, requeue and purge. They require the `messaging-admin`
  policy, which accepts an API key in the `X-Admin-Key` header (`Admin:ApiKey`; `local-dev-admin-key` in
  development), and answer 401 without it. See `GM.OTP.Sample.API/Admin/MessagingAdmin.cs`.
- **Inbox → OTP** — the consumer worker stores each incoming request with `IngestAsync`, and the
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
