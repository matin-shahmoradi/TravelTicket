# Payment (`src/Services/Payment/Payment.api`)
**Responsibility:** create payment for each order, redirect to ZarinPal, verify callback, publish result.
**Endpoints** (NOT routed through Gateway):
- GET `api/payments/orders/{orderId}/pay-url` - redirects to gateway URL
- GET `api/payments/callback?paymentId&authority&status` - verifies via ZarinPal, returns result
(`Features/CallBack/PaymentEndpoints.cs`)
**Models:** `Domain/PaymentModel.cs` aggregate (OrderId, CustomerId, Amount, Status, Authority, TransactionId, FailureReason, CreatedAtUtc, PaidAtUtc; `MarkPending/MarkGatewayRequested/MarkSucceeded/MarkFailed`). `PaymentStatus`: None, Pending, GatewayRequested, Succeeded, Failed, Cancelled.
**Events received:** OrderCreatedIntegrationEvent -> CreatePaymentCommand (`Consumers/OrderPaymentRequestedConsumer.cs`, swallows exceptions).
**Events sent:** PaymentSucceeded/FailedIntegrationEvent via direct `bus.Publish` (`EventHandlers/*`), not outbox.
**External:** ZarinPal sandbox (`Services/ZarinPalGateway.cs`, `Options/ZarinPalOptions.cs`); appsettings holds sandbox MerchantId and CallbackUrl `https://localhost:7265/...`.
**Run:** compose ports 6090/6091, DB `PaymentDb` (5450), `Data/MigrateDb.cs`.
**Test:** `src/Services/Payment/Tests/TestProject1` (PaymentService.Test.csproj) - no test classes found.
**Unknowns:** auth on endpoints (none seen); callback URL vs compose port; migration apply details.
