using BuildingBlocks.DDD;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Catalog.API.Data.Interceptors
{
    internal sealed class DispatchEventInterceptor(Mediator mediator) : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            return base.SavingChanges(eventData, result);
        }
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private async Task DispatchCatalogDomainEvents(CatalogDbContext? catalogDbContext)
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
