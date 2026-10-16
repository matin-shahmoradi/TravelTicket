# Flow: Ticket create/update -> integration events -> Basket
Services: Catalog -> (RabbitMQ) -> Basket. Gateway prefix `/tickets`.

**Create**
1. `POST /tickets` (AdminOnly) - `Catalog/Catalog.API/Tickets/CreateTicket/CreateTicketEndpoint.cs` -> `CreateTicketCommand(CreateTicketRequestDTO{Origin, Destination, Description, Date, Price})`. DTO and handler share `CreateTicketHandler.cs`; validator in `CreateTicketCommandValidator.cs`.
2. `CreateTicketCommandHandler`: `Ticket.Create(TicketId.New(), ...)` (`Domain/Ticket.cs:52` adds `TicketCreatedEvent`), `ITicketCommandRepository.AddTicketAsync`, `IUnitOfWork.SaveChangesAsync`. Returns 201 with the new Guid.
3. In `SavingChanges` (BEFORE the DB write), `DispatchDomainEventInterceptor` (BuildingBlocks) publishes the aggregate's domain events via MediatR, then clears them.
4. `EventHandlers/TicketCreatedEventHandler.cs` builds `TicketCreatedIntegrationEvent` and calls `repository.PublishIntegrationEvent` (`Repository/TicketCommandRepository.cs`) -> `context.OutboxMessages.AddIntegrationEvent(...)`, then calls `SaveChangesAsync` (nested; events already cleared so no recursion). The ticket and the outbox row are written by that nested save.
5. Outbox job (`BuildingBlocks/Infrastracture/Outbox/OutboxProcessor.cs`) publishes to RabbitMQ.
6. Basket consumer `Basket.API/EventHandlers/CatalogEvents/TicketCreatedEventConsumer.cs` handles it (effect UNKNOWN: body unread).

**Update**
1. `PUT tickets/{id:guid}` (AdminOnly) - `Tickets/UpdateTicket/UpdateTicketEndpoint.cs` -> `UpdateTicketCommand(Id, UpdateTicketRequestDTO)`; all fields optional (partial update).
2. `UpdateTicketCommandHandler.cs`: load via `ITicketQueryRepository`, apply `ChangeOrigin/Destination/Description/Price/TravelDate` only for non-null fields; each raises `TicketUpdatedEvent` (`Domain/Ticket.cs:71-114`), price change also `TicketPriceChangedEvent` (`Ticket.cs:75`). Missing ticket -> NotFound.
3. `UpdateTicket` + `SaveChangesAsync` -> interceptor -> `EventHandlers/*` -> outbox -> RabbitMQ.
4. Basket consumer `EventHandlers/CatalogEvents/TicketPriceChangedEventConsumer.cs` reacts to price changes (effect UNKNOWN).

**Delete**: `DELETE tickets/{id}` -> `Tickets/DeleteTicket/` -> `EventHandlers/TicketDeletedEventHandler.cs` -> outbox.

Notes:
- The handler runs before the outer write and does a nested save, so entity and outbox row are saved together. If the handler throws, it is caught and logged, and the event is silently lost while the ticket is still saved by the outer call.
- Several changes in one update add multiple `TicketUpdatedEvent`s (duplicate events possible).
- Tests: `Catalog.tests/Catalog.UnitTest/Features/{CreateTicket,UpdateTicket}`, `Catalog.IntegrationTest/Features/{CreateTicketIntegrationTest,UpdateTicketIntegrationEvent}.cs` (Testcontainers, needs Docker).
