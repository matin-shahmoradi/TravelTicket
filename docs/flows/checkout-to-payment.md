# Flow: Basket checkout -> Order -> Payment -> Order status
Services: Basket -> (RabbitMQ) -> Ordering -> (RabbitMQ) -> Payment -> (RabbitMQ) -> Ordering. Domain events are dispatched in `SavingChanges` (before the DB write) by `src/Commons/BuildingBlocks/EntityFramwork/Interceptors/DispatchDomainEventInterceptor.cs`. Its method is named `DispatchCatalogDomainEvents` but it is shared; which services register it is UNKNOWN (Catalog/Basket/Ordering/Payment extension files reference it).

**A. Basket (checkout)**
1. `POST baskets/checkout` - `Basket/Basket.API/basket/BasketCheckout/BasketCheckOutEndpoint.cs` -> `BasketCheckOutCommand`.
2. `BasketCheckOutCommandHandler.cs` loads the user's basket (`ICurrentUser.UserId`), calls `ShoppingCart.CheckoutBasket()` (`Model/ShoppingCart.cs:60`, adds `BasketCheckOutEvent`), then `SaveChangesAsync`.
3. The interceptor publishes the domain event -> `EventHandlers/BasketEvents/BasketCheckOutEventHandler.cs` builds `BasketCheckOutIntegrationEvent` (customer id, email, phone, items) and writes it to the `outboxMessages` table, then saves.
4. Hangfire job `OutboxProcessor<TContext>` (`BuildingBlocks/Infrastracture/Outbox/OutboxProcessor.cs`) reads rows with `ProcessedOnUtc == null`, `IPublishEndpoint.Publish`es them, sets `ProcessedOnUtc`.

**B. Ordering (create order)**
5. `Ordering.Infrastructure/Consumers/BasketCheckOutEventConsumer.cs` (namespace `Ordering.Application.Events`) builds `OrderDto` (status Pending; national code hardcoded "UNKWNOWN") and sends `CreateOrderCommand`.
6. `Order.cs:27` `Order.Create` adds `OrderCreatedEvent` -> `Ordering.Application/EventHandlers/OrderCreatedEventHandler.cs` writes `OrderCreatedIntegrationEvent` (Amount = `TotalPrice`) to the Ordering outbox -> published by the outbox job.

**C. Payment (request)**
7. `Payment.api/Consumers/OrderPaymentRequestedConsumer.cs` consumes `OrderCreatedIntegrationEvent` -> `CreatePaymentCommand`.
8. `Features/PaymentRequest/CreatePaymentCommandHandler.cs` creates `PaymentModel` and calls `ZarinPalGateway.RequestPaymentAsync` (`Services/ZarinPalGateway.cs`). Gateway failure -> `MarkFailed`; success -> `MarkGatewayRequested(authority)`.

**D. Client pays**
9. `GET api/payments/orders/{orderId}/pay-url` - `Features/CallBack/PaymentEndpoints.cs` -> `GetPaymentUrlQueryHandler` builds `PaymentPageUrl + Authority` -> HTTP redirect to ZarinPal. The client must wait for step 8 to finish, else 404 (payment not found).
10. ZarinPal redirects to `GET api/payments/callback?paymentId&authority&status`.
11. `VerifyPaymentCommandHandler.cs`:
   - Already Succeeded -> idempotent success.
   - `status != "OK"` -> `MarkFailed("User cancelled payment")`.
   - Else `VerifyPaymentAsync` with ZarinPal; failure -> `MarkFailed`; success -> `MarkSucceeded(authority, transactionId, paidAtUtc)`.
   - Runs in `ITransactionExecutor`.

**E. Back to Ordering**
12. `PaymentModel` raises `PaymentSucceededDomainEvent` / `PaymentFailedDomainEvent` (`Domain/PaymentModel.cs:94,114`) -> `Payment.api/EventHandlers/Payment{Succeeded,Failed}EventHandler.cs` call `bus.Publish` directly (NO outbox: a broker outage loses the event).
13. `Ordering.Infrastructure/Consumers/Payment{Succeeded,Failed}EventConsumer.cs` -> `UpdateOrderCommand` -> `Orders/Commands/UpdateOrder/UpdateOrderCommandHandler.cs`: finds the order, maps payment status via `IOrderStatusFactory.CreateFromPaymentStatus` (mapping UNKNOWN), `Order` adds `OrderUpdatedEvent`.

Risks:
- Basket is not cleared in the handler shown (`BasketCheckOutCommandHandler.cs`); whether `CheckoutBasket()` empties it is UNKNOWN.
- Handlers catch and log exceptions after the outbox write; failures are swallowed.
- Ordering consumer failure only logs and returns (no retry/dead-letter visible).
- Tests: `Basket/Test/Basket.UnitTests/Features/BasketCheckout`, `Basket.IntegrationTests/Features/BasketCheckoutIntegerationTest.cs`. No Ordering or Payment flow tests found.
