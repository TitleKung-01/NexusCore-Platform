namespace NexusCore.Infrastructure.Notifications;

public class N8nOptions
{
    public const string SectionName = "N8n";
    public string? WebhookUrl { get; set; }
    public string? WebhookSecret { get; set; }
}
