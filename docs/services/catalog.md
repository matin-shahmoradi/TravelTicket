# Catalog (`src/Services/Catalog/Catalog.API`)
**Responsibility:** ticket (route/date/price) CRUD, gRPC lookup for Basket, publishes ticket integration events via outbox.
**Endpoints** (Gateway prefix `/tickets`):
- GET `tickets?pageNumber&pageSize` - paged list
- GET `tickets/{id:guid}`
- POST `/tickets` - AdminOnly; TicketRequestDTO (`Common/DTOs/TicketRequestDTO.cs`)
- PUT `tickets/{id:guid}` - AdminOnly
- DELETE `tickets/{id}` - AdminOnly
- gRPC `CatalogGrpcService.GetTicketById` (`Grpc/CatalogRpcService.cs`, proto in BuildingBlocks `Contracts/Protos/Catalog.proto`)
**Trip aggregate (new, will replace Ticket; no handlers/endpoints/migration yet):** `Domain/Trip.cs` (`Aggregate<TripId>`; Draft->OnSale->SalesClosed->Departed|Cancelled), entities `Domain/Entities/{Seat,Reservation,ReservationSeat}.cs`, value-equality records in `Domain/ValueObjects/` (Money>0, SeatNumber 1-60, TripRoute, TravelSchedule, BusInfo; max capacity VIP 15, Sleeper 20, Standard 60). Seat flow: `HoldSeats` (idempotent per order, TTL<=10 min) -> `ConfirmReservation` | `ReleaseReservation` | `ExpireHolds`; `Cancel` emits `ReservationCancelledEvent` per reservation (`WasConfirmed` => refund) plus `TripCancelledEvent`. Events in `Events/{TripEvents,SeatEvents}`; EF configs in `Data/EntityConfigurations/{Trip,Seat,Reservation,ReservationSeat}Configuration.cs`. TODO: migration (dotnet-ef not installed), repositories, handlers, endpoints, integration events. `Ticket` intentionally untouched.
**Models (legacy):** `Domain/Ticket.cs` aggregate (Origin, Destination, Description, TravelDate, Price; private setters, `Create/Update/Change*`), strongly-typed `TicketId`. DbContext `Data/CatalogDbContext.cs`, seed `Data/CatalogInitialData.cs` (run in Development via `app.Populate()`).
**Events sent:** Ticket{Created,Updated,Deleted,PriceChanged}IntegrationEvent (domain events -> `EventHandlers/*` -> outbox -> RabbitMQ). Received: none.
**Config:** `JwtSetting:Key|Issuer|Audience` (note: different section name than Auth's `Jwt`).
**Run:** compose port 6000, health `/health-catalog`, Hangfire dashboard (unauthenticated), Swagger in Development.
**Test:** `src/Services/Catalog/Catalog.tests/` - UnitTest (domain, handlers, validators) and IntegrationTest (Testcontainers via `CatalogIntegrationFactory.cs`; needs Docker).
**Unknowns:** TicketRequestDTO fields; exact outbox write path (`Repository/TicketCommandRepository.cs`).
