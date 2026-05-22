namespace NexusCore.Application.Announcements;

/// <summary>ประกาศข่าวสารภายในองค์กร</summary>
public record AnnouncementResponse(Guid Id, string Title, string Body, Guid AuthorId, string PublishedAtUtc, bool IsActive);

/// <summary>คำขอสร้างประกาศใหม่</summary>
public record CreateAnnouncementRequest(string Title, string Body);

/// <summary>คำขอแก้ไขประกาศและสถานะเผยแพร่</summary>
public record UpdateAnnouncementRequest(string Title, string Body, bool IsActive);
