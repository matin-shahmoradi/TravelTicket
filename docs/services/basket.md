# Basket (`src/Services/Basket/Basket.API`)
**Responsibility:** per-user shopping cart (Postgres `BasketDb`), Redis cache of ticket read models, checkout trigger.
**Endpoints** (Gateway prefix `/baskets`; folder is lowercase `basket/`):
- GET `/baskets` - policy "User"
- POST `baskets` - store whole basket (BasketRequest)
- POST `baskets/items` - add item; fetches ticket via gRPC `ICatalogGrpcClient` (`Grpc/GrpcClients/CatalogGrpcClient.cs`)
- DELETE `baskets/items`
- GET `baskets/items/{TicketId}` - read ticket from cache
- POST `/baskets/checkout` - no body; `BasketCheckOutCommandHandler` loads basket, calls `CheckoutBasket()`, saves.
**Models:** ShoppingCart + ShoppingCartItem (`Data/EntityConfigurations/`), strongly-typed id converters in `Data/ValueConverters/`; repos `Data/Repositories/` (`CachedBasketRepository` decorates `BasketRepository`).
**Events:** receives TicketCreated, TicketPriceChanged (`EventHandlers/CatalogEvents/`) -> updates Redis/prices. Sends BasketCheckOutIntegrationEvent (domain event `BasketCheckOutEventHandler.cs` -> outbox).
**Calls:** gRPC Catalog (address in `BasketExtension.cs:90`).
**Run:** compose port 6003/6053 (needs `src/certs/dev-cert.pfx`, `${CertPassword}`), health `/health-basket`, Redis `ConnectionStrings:Redis`.
**Test:** `src/Services/Basket/Test/Basket.UnitTests` and `Basket.IntegrationTests` (Docker). 
**Unknowns:** cache TTL; whether basket is cleared after checkout/payment (TODO.cs says pending).
