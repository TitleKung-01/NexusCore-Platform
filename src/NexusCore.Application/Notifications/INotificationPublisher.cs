namespace NexusCore.Application.Notifications;

public interface INotificationPublisher
{
    Task PublishAsync(HrEventPayload payload, CancellationToken cancellationToken = default);
}
