using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MechanicShop.Infrastructure.Data.Interceptors;

public class AuditableEntityInterceptor(IUser user, TimeProvider timeProvider)
    : SaveChangesInterceptor
{
    private readonly IUser _user = user;
    private readonly TimeProvider _timeProvider = timeProvider;

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result
    )
    {
        UpdateAuditableEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        UpdateAuditableEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateAuditableEntities(DbContext? context)
    {
        if (context is null)
            return;

        var utcNow = _timeProvider.GetUtcNow();
        string? userId = _user.UserId;
        foreach (var entry in context.ChangeTracker.Entries<AuditableEntity>())
        {
            if (
                entry.State is EntityState.Added or EntityState.Modified
                || entry.References.Any(r =>
                    r.TargetEntry?.Metadata.IsOwned() == true
                    && (r.TargetEntry.State is EntityState.Modified or EntityState.Added)
                )
            )
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.CreatedAtUtc = utcNow;
                }
                entry.Entity.LastModifiedUtc = utcNow;
                entry.Entity.LastModifiedBy = _user.UserId;
                UpdateOwnedEntities(entry, utcNow, userId);
            }
        }
    }

    private static void UpdateOwnedEntities(
        EntityEntry<AuditableEntity> entry,
        DateTimeOffset utcNow,
        string? userId
    )
    {
        foreach (ReferenceEntry ownedEntry in entry.References)
        {
            if (
                ownedEntry.TargetEntry is { Entity: AuditableEntity ownedEntity }
                && ownedEntry.TargetEntry.State is EntityState.Added or EntityState.Modified
            )
            {
                if (ownedEntry.TargetEntry.State == EntityState.Added)
                {
                    ownedEntity.CreatedAtUtc = utcNow;
                    ownedEntity.CreatedBy = userId;
                }
                ownedEntity.LastModifiedBy = userId;
                ownedEntity.LastModifiedUtc = utcNow;
            }
        }
    }
}

public static class Extensions
{
    public static bool HasChangedOwnedEntities(
        this Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry
    )
    {
        return entry.References.Any(r =>
            r.TargetEntry?.Metadata.IsOwned() == true
            && (r.TargetEntry.State is EntityState.Added or EntityState.Modified)
        );
    }
}
