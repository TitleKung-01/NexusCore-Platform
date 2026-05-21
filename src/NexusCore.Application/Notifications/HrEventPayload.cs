namespace NexusCore.Application.Notifications;

public record HrEventPayload(
    string EventType,
    string? RecipientEmail,
    string Subject,
    string Body,
    object? Metadata = null);
