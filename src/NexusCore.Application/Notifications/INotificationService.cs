namespace NexusCore.Application.Notifications;

public interface INotificationService
{
    Task<IReadOnlyList<NotificationResponse>> ListAsync(CancellationToken cancellationToken = default);
    Task<UnreadCountResponse> GetUnreadCountAsync(CancellationToken cancellationToken = default);
    Task<bool> MarkReadAsync(Guid id, CancellationToken cancellationToken = default);
    Task MarkAllReadAsync(CancellationToken cancellationToken = default);
    Task NotifyUserAsync(
        Guid userId,
        string eventType,
        string title,
        string body,
        string? linkPath,
        string? recipientEmail,
        string? emailSubject,
        CancellationToken cancellationToken = default);
}
