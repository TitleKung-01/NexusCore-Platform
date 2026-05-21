using NexusCore.Domain.Entities;

namespace NexusCore.Domain.Interfaces;

public interface IAppNotificationRepository
{
    Task<IReadOnlyList<AppNotification>> GetForUserAsync(Guid userId, int limit, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(AppNotification notification, CancellationToken cancellationToken = default);
    Task<AppNotification?> FindByIdTrackedAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    Task MarkAllReadAsync(Guid userId, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
