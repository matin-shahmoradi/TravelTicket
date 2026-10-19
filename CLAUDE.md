# TravelTicket - AI agent guide
.NET 10 microservices for ticket sales (Auth, Catalog, Basket, Ordering, Payment, Gateway). Solution: `src/TravelTicket-microservice.slnx`.

## Read first (in this order, stop when you have enough)
- `ARCHITECTURE.md` - purpose, communication diagram, top-10 files, risks, UNKNOWNs
- `docs/services/{auth,catalog,basket,ordering,payment,gateway,buildingblocks}.md` - one per service
- `docs/flows/auth-register-login.md`, `docs/flows/checkout-to-payment.md`, `docs/flows/ticket-create-update.md`

## Commands (run from `src/`)
- Build: `dotnet build TravelTicket-microservice.slnx`
- Test all: `dotnet test TravelTicket-microservice.slnx` (Catalog/Basket integration tests need Docker/Testcontainers)
- Test one project: `dotnet test <path-to.csproj>`
- Run all: `docker-compose up` (needs untracked `src/.env` and `src/certs/dev-cert.pfx`)
- Run one service: `dotnet run` in its API project dir
- Ports (compose): catalog 6000, basket 6003/6053, auth 6066, ordering 6080, payment 6090/6091

## Conventions
- Vertical slices: `<Feature>/{Command,Handler,Endpoint}`; MediatR CQRS (`ICommand`/`IQuery`, `ICommandHandler`).
- Endpoints are Carter `ICarterModule`; FluentValidation validators sit next to commands.
- Handlers return `Result<T>`; map to HTTP with `ToHttpResult(httpContext)` (BuildingBlocks).
- DDD: aggregates call `AddDomainEvents`; `DispatchDomainEventInterceptor` publishes them during `SavingChanges`.
- Integration events go through the Outbox (`OutboxMessages.AddIntegrationEvent`, Hangfire `OutboxProcessor`) then MassTransit/RabbitMQ.
- Strongly-typed IDs (e.g. `TicketId`) with EF value converters. Use `IUnitOfWork`/`ITransactionExecutor`.
- Shared code lives in `src/Commons/BuildingBlocks`; contracts (events, protos) are there too.
- Basket -> Catalog sync call is gRPC (`Contracts/Protos/Catalog.proto`).

## Warnings
- Keep typo'd folder names `Infrastracture` and `EntityFramwork`; renaming breaks namespaces.
- Payment publishes `PaymentSucceeded/Failed` via `bus.Publish`, NOT the outbox (events can be lost).
- Event handlers catch and log exceptions after outbox writes; failures are silent.
- JWT config differs: Auth uses `Jwt:*`, Catalog uses `JwtSetting:*`; keys must match across services.
- Gateway ports do not match compose ports; verify before relying on Gateway routes.
- Hangfire dashboard is unauthenticated; Auth SMTP port 2526 (appsettings) vs 2525 (compose).
- Basket folder casing differs (`Basket/` vs `basket/`); mind it on case-sensitive systems.
- Never commit `.env` or real ZarinPal/JWT secrets; the Payment MerchantId in appsettings is a sandbox value.
- Ordering hardcodes customer national code as "UNKWNOWN"; no tests for Ordering/Payment flows.
- Do not edit `Migrations/` by hand; migration strategy is UNKNOWN.

## Working tips
- Prefer reading a service doc, then only the feature folder you will change.
- If a doc says UNKNOWN, read the cited file rather than guessing; update the doc afterwards.
- Catalog domain is moving from `Ticket` to the `Trip` aggregate (seats/reservations, see docs/services/catalog.md). Do not modify `Ticket` until other services are refactored.
