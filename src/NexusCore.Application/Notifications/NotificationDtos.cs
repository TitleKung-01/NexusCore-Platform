namespace NexusCore.Application.Notifications;

/// <summary>การแจ้งเตือนในแอปสำหรับผู้ใช้</summary>
public record NotificationResponse(
    Guid Id,
    string EventType,
    string Title,
    string Body,
    string? LinkPath,
    bool IsRead,
    string CreatedAtUtc);

/// <summary>จำนวนการแจ้งเตือนที่ยังไม่อ่าน</summary>
public record UnreadCountResponse(int Count);
