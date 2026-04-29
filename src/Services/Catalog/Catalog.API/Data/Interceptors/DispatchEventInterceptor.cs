using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Catalog.API.Data.Interceptors
{
    internal sealed class DispatchEventInterceptor(IMediator mediator) : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            DispatchCatalogDomainEvents(eventData.Context).GetAwaiter().GetResult();
            return base.SavingChanges(eventData, result);
        }
        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            await DispatchCatalogDomainEvents(eventData.Context);
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private async Task DispatchCatalogDomainEvents(DbContext? catalogDbContext)
        {
            if(catalogDbContext == null) return;

            var aggregates = catalogDbContext.ChangeTracker
                .Entries<IAggregate>()
                .Where(ag => ag.Entity.DomainEvents.Any())
                .Select(a => a.Entity);

            var domainEvents = aggregates
                .SelectMany(de => de.DomainEvents)
                .ToList();

            aggregates.ToList().ForEach(a => a.ClearDomainEvent());

            foreach(var domainEvent in domainEvents)
            {
                await mediator.Publish(domainEvent);
            }
        }
    }
}
