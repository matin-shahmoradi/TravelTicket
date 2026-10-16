# Catalog (`src/Services/Catalog/Catalog.API`)
**Responsibility:** ticket (route/date/price) CRUD, gRPC lookup for Basket, publishes ticket integration events via outbox.
**Endpoints** (Gateway prefix `/tickets`):
- GET `tickets?pageNumber&pageSize` - paged list
- GET `tickets/{id:guid}`
- POST `/tickets` - AdminOnly; TicketRequestDTO (`Common/DTOs/TicketRequestDTO.cs`)
- PUT `tickets/{id:guid}` - AdminOnly
- DELETE `tickets/{id}` - AdminOnly
- gRPC `CatalogGrpcService.GetTicketById` (`Grpc/CatalogRpcService.cs`, proto in BuildingBlocks `Contracts/Protos/Catalog.proto`)
**Models:** `Domain/Ticket.cs` aggregate (Origin, Destination, Description, TravelDate, Price; private setters, `Create/Update/Change*`), strongly-typed `TicketId`. DbContext `Data/CatalogDbContext.cs`, seed `Data/CatalogInitialData.cs` (run in Development via `app.Populate()`).
**Events sent:** Ticket{Created,Updated,Deleted,PriceChanged}IntegrationEvent (domain events -> `EventHandlers/*` -> outbox -> RabbitMQ). Received: none.
**Config:** `JwtSetting:Key|Issuer|Audience` (note: different section name than Auth's `Jwt`).
**Run:** compose port 6000, health `/health-catalog`, Hangfire dashboard (unauthenticated), Swagger in Development.
**Test:** `src/Services/Catalog/Catalog.tests/` - UnitTest (domain, handlers, validators) and IntegrationTest (Testcontainers via `CatalogIntegrationFactory.cs`; needs Docker).
**Unknowns:** TicketRequestDTO fields; exact outbox write path (`Repository/TicketCommandRepository.cs`).
