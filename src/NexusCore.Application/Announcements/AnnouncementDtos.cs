namespace NexusCore.Application.Announcements;

public record AnnouncementResponse(Guid Id, string Title, string Body, Guid AuthorId, string PublishedAtUtc, bool IsActive);

public record CreateAnnouncementRequest(string Title, string Body);

public record UpdateAnnouncementRequest(string Title, string Body, bool IsActive);
