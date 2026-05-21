using NexusCore.Application.Common;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Application.Notifications;

public class NotificationService(
    ICurrentUserService currentUser,
    IAppNotificationRepository notifications) : INotificationService
{
    public async Task<IReadOnlyList<NotificationResponse>> ListAsync(CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is null)
            return [];

        var list = await notifications.GetForUserAsync(currentUser.UserId.Value, 50, cancellationToken);
        return list.Select(Map).ToList();
    }

    public async Task<UnreadCountResponse> GetUnreadCountAsync(CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is null)
            return new UnreadCountResponse(0);

        var count = await notifications.GetUnreadCountAsync(currentUser.UserId.Value, cancellationToken);
        return new UnreadCountResponse(count);
    }

    public async Task<bool> MarkReadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is null)
            return false;

        var n = await notifications.FindByIdTrackedAsync(id, currentUser.UserId.Value, cancellationToken);
        if (n is null)
            return false;

        n.IsRead = true;
        await notifications.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task MarkAllReadAsync(CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is null)
            return;

        await notifications.MarkAllReadAsync(currentUser.UserId.Value, cancellationToken);
    }

    public async Task NotifyUserAsync(
        Guid userId,
        string eventType,
        string title,
        string body,
        string? linkPath,
        CancellationToken cancellationToken = default)
    {
        await notifications.AddAsync(new AppNotification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            EventType = eventType,
            Title = title,
            Body = body,
            LinkPath = linkPath,
            IsRead = false,
            CreatedAtUtc = DateTime.UtcNow
        }, cancellationToken);
        await notifications.SaveChangesAsync(cancellationToken);
    }

    private static NotificationResponse Map(AppNotification n) =>
        new(n.Id, n.EventType, n.Title, n.Body, n.LinkPath, n.IsRead, n.CreatedAtUtc.ToString("o"));
}
