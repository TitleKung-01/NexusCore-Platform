namespace NexusCore.Application.Notifications;

/// <summary>
/// บริการแจ้งเตือนในแอป: อ่าน ทำเครื่องหมาย และส่งให้ผู้ใช้
/// </summary>
public interface INotificationService
{
    /// <summary>รายการแจ้งเตือนล่าสุดของผู้ล็อกอิน</summary>
    Task<IReadOnlyList<NotificationResponse>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>นับจำนวนที่ยังไม่อ่าน</summary>
    Task<UnreadCountResponse> GetUnreadCountAsync(CancellationToken cancellationToken = default);

    /// <summary>ทำเครื่องหมายแจ้งเตือนเดียวว่าอ่านแล้ว</summary>
    Task<bool> MarkReadAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ทำเครื่องหมายทั้งหมดว่าอ่านแล้ว</summary>
    Task MarkAllReadAsync(CancellationToken cancellationToken = default);

    /// <summary>สร้างแจ้งเตือนให้ผู้ใช้ (ใช้จาก use case อื่น)</summary>
    Task NotifyUserAsync(
        Guid userId,
        string eventType,
        string title,
        string body,
        string? linkPath,
        CancellationToken cancellationToken = default);
}
