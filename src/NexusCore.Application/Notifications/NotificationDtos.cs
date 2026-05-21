namespace NexusCore.Application.Notifications;

public record NotificationResponse(
    Guid Id,
    string EventType,
    string Title,
    string Body,
    string? LinkPath,
    bool IsRead,
    string CreatedAtUtc);

public record UnreadCountResponse(int Count);
