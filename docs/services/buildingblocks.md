# BuildingBlocks (`src/Commons/BuildingBlocks`)
**Responsibility:** shared library referenced by every service (net10.0; also pulls test libs - risk).
**Contents:** `CQRS/` (ICommand/IQuery + handlers), `Behaviors/` (Validation, Logging, ExceptionLogging MediatR pipeline), `DDD/` (Entity, Aggregate, IDomainEvent), `ResultLib/` (Result, Error, ErrorType), `EntityFramwork/` (UnitOfWork, EfTransactionExecutor, AuditInterceptor, DispatchDomainEventInterceptor), `Infrastracture/Outbox/` (OutboxMessage, OutboxProcessor<TContext>, Hangfire recurring job via `UseBackgroundJobs("<name>")`, `AddIntegrationEvent` extension), `Messaging/Events/` (IntegrationEvent base, all events, `AddMassTransitWithAssembly`), `Contracts/Protos/Catalog.proto`, `CorrelationId/`, `OpenTelemetry/`, `Serilog/` (sensitive data masking), `Pagination/`, `CustomExceptions/CustomExceptionHandler.cs`, `Extensions/HttpExtensions.cs` (`Result.ToHttpResult`), `TestBase/` (CustomWebApplicationFactory, fake JWT).
**Integration events:** BasketCheckOut, TicketCreated/Updated/Deleted/PriceChanged, OrderCreated, PaymentSucceeded/Failed.
**Broker config keys:** `MessageBroker:Host|UserName|Password`.
**Test:** none directly; consumed via service tests.
**Unknowns:** outbox per-service table setup (`OutboxMessageBaseEntityConfiguration.cs`); Hangfire storage config.
