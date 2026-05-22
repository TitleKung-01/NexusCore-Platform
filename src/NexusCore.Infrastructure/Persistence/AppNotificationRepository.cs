using Microsoft.EntityFrameworkCore;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Infrastructure.Persistence;

/// <summary>เข้าถึงการแจ้งเตือนในแอป (AppNotifications)</summary>
public class AppNotificationRepository(AppDbContext db) : IAppNotificationRepository
{
    /// <summary>ดึงการแจ้งเตือนล่าสุดของผู้ใช้</summary>
    public async Task<IReadOnlyList<AppNotification>> GetForUserAsync(Guid userId, int limit, CancellationToken cancellationToken = default) =>
        await db.AppNotifications
            .AsNoTracking()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAtUtc)
            .Take(limit)
            .ToListAsync(cancellationToken);

    /// <summary>นับจำนวนการแจ้งเตือนที่ยังไม่อ่าน</summary>
    public Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default) =>
        db.AppNotifications.CountAsync(n => n.UserId == userId && !n.IsRead, cancellationToken);

    /// <summary>เพิ่มการแจ้งเตือนใหม่</summary>
    public async Task AddAsync(AppNotification notification, CancellationToken cancellationToken = default) =>
        await db.AppNotifications.AddAsync(notification, cancellationToken);

    /// <summary>ค้นหาการแจ้งเตือนของผู้ใช้ (ติดตามการเปลี่ยนแปลง)</summary>
    public Task<AppNotification?> FindByIdTrackedAsync(Guid id, Guid userId, CancellationToken cancellationToken = default) =>
        db.AppNotifications.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId, cancellationToken);

    /// <summary>ทำเครื่องหมายว่าอ่านทั้งหมดแล้ว</summary>
    public async Task MarkAllReadAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await db.AppNotifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true), cancellationToken);
    }

    /// <summary>บันทึกการเปลี่ยนแปลงลงฐานข้อมูล</summary>
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
