using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NexusCore.Application.Notifications;

namespace NexusCore.Infrastructure.Notifications;

public class N8nWebhookNotificationPublisher(
    IHttpClientFactory httpClientFactory,
    IOptions<N8nOptions> options,
    ILogger<N8nWebhookNotificationPublisher> logger) : INotificationPublisher
{
    public async Task PublishAsync(HrEventPayload payload, CancellationToken cancellationToken = default)
    {
        var url = options.Value.WebhookUrl;
        if (string.IsNullOrWhiteSpace(url))
            return;

        try
        {
            var client = httpClientFactory.CreateClient("n8n");
            using var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = JsonContent.Create(payload)
            };

            if (!string.IsNullOrWhiteSpace(options.Value.WebhookSecret))
                request.Headers.TryAddWithoutValidation("X-HR-Lite-Secret", options.Value.WebhookSecret);

            var response = await client.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(cancellationToken);
                logger.LogWarning("n8n webhook returned {Status}: {Body}", response.StatusCode, body);
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to publish n8n event {EventType}", payload.EventType);
        }
    }
}
