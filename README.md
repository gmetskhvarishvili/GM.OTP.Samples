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
├── GM.OTP.Sample.Worker          # Background processing
└── tests/
    └── GM.OTP.Sample.Tests       # command-validator, event, and aggregate unit tests
```

Only the published **GM.*** packages are referenced — no project references into the library repos.

## What it demonstrates

- **OTP generation & verification** — `GenerateOtp` issues a salted, subject/destination-bound code
  via `GM.OTP`'s `OtpManager` and persists an `OtpChallenge`; `VerifyOtp` checks a submitted code.
- **Outbox → messaging** — the generated code is published as an `OtpGeneratedIntegrationEvent`
  through the GM.Messaging outbox for a downstream service to deliver.
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
