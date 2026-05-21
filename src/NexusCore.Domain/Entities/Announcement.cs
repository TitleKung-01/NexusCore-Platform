namespace NexusCore.Domain.Entities;

public class Announcement
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public DateTime PublishedAtUtc { get; set; }
    public bool IsActive { get; set; } = true;
}
