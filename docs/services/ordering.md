# Ordering (`src/Services/Ordering/Ordering.{API,Application,Domain,Infrastructure}`)
**Responsibility:** create orders from basket checkout events, track status from payment events, emit OrderCreated.
**Endpoints:** GET `/orders/{id:guid}` only (`Ordering.API/Controllers/OrderEndpoints.cs`). `WeatherForecastController` is template leftover.
**Layers:** Domain (`Models/Order.cs` aggregate: CustomerId, OrderStatus, OrderItems, TotalPrice; `Create/UpdateOrderStatus/Add/Remove`; `Enums/OrderStatus` Draft=1,Pending=2,Completed=3,Cancelled=4). Application (MediatR commands Create/Update/Delete, query GetOrderById, `EventHandlers/OrderCreatedEventHandler.cs`). Infrastructure (`Data/OrderDbContext.cs`, `Consumers/*`, `DependencyInjection.cs`).
**Events received:** BasketCheckOutIntegrationEvent -> CreateOrderCommand (status Pending; customer nationalCode hardcoded "UNKWNOWN"); PaymentSucceeded / PaymentFailed -> UpdateOrderCommand (exceptions caught and only logged).
**Events sent:** OrderCreatedIntegrationEvent via outbox (`OrderCreatedEventHandler.cs:25`).
**Run:** compose port 6080, DB `OrderingDb` (5435); `InitialiseDatabaseAsync()` at startup (`Program.cs:38`).
**Test:** no Ordering test project found.
**Unknowns:** how PaymentStatus string maps to OrderStatus (`Factories/OrderStatusFactory`); JWT config; Update/Delete have no endpoints.
