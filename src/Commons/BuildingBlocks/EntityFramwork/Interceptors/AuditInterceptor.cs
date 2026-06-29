using BuildingBlocks.Abstractions;
using BuildingBlocks.DDD;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BuildingBlocks.EntityFramwork.Interceptors
{
    public sealed class AuditInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUser _currentUser;
        public AuditInterceptor(ICurrentUser currentUser)
        {
            _currentUser = currentUser;
        }
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateEntities(eventData.Context);
            return base.SavingChanges(eventData, result);
        }
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            UpdateEntities(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void UpdateEntities(DbContext? context)
        {
            if (context == null) return;

            var userId = _currentUser.IsAuthenticated ? _currentUser.UserId.ToString() : "System";
            foreach (var entry in context.ChangeTracker.Entries<IEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = userId;
                }
                if (entry.State == EntityState.Modified || entry.State == EntityState.Added)
                {
                    entry.Entity.LastModified = DateTime.UtcNow;
                    entry.Entity.LastModifiedBy = userId;
                }
            }
        }
    }
}
