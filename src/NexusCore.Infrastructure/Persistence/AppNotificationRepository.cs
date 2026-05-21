using Microsoft.EntityFrameworkCore;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Infrastructure.Persistence;

public class AppNotificationRepository(AppDbContext db) : IAppNotificationRepository
{
    public async Task<IReadOnlyList<AppNotification>> GetForUserAsync(Guid userId, int limit, CancellationToken cancellationToken = default) =>
        await db.AppNotifications
            .AsNoTracking()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAtUtc)
            .Take(limit)
            .ToListAsync(cancellationToken);

    public Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default) =>
        db.AppNotifications.CountAsync(n => n.UserId == userId && !n.IsRead, cancellationToken);

    public async Task AddAsync(AppNotification notification, CancellationToken cancellationToken = default) =>
        await db.AppNotifications.AddAsync(notification, cancellationToken);

    public Task<AppNotification?> FindByIdTrackedAsync(Guid id, Guid userId, CancellationToken cancellationToken = default) =>
        db.AppNotifications.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId, cancellationToken);

    public async Task MarkAllReadAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await db.AppNotifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true), cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
