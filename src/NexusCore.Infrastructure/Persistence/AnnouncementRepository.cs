using Microsoft.EntityFrameworkCore;
using NexusCore.Domain.Entities;
using NexusCore.Domain.Interfaces;

namespace NexusCore.Infrastructure.Persistence;

/// <summary>เข้าถึงข้อมูลประกาศบริษัท (Announcements)</summary>
public class AnnouncementRepository(AppDbContext db) : IAnnouncementRepository
{
    /// <summary>ดึงประกาศที่เปิดใช้งาน เรียงตามวันเผยแพร่ล่าสุด</summary>
    public async Task<IReadOnlyList<Announcement>> ListActiveAsync(CancellationToken cancellationToken = default) =>
        await db.Announcements
            .AsNoTracking()
            .Where(a => a.IsActive)
            .OrderByDescending(a => a.PublishedAtUtc)
            .ToListAsync(cancellationToken);

    /// <summary>ค้นหาประกาศตามรหัส (อ่านอย่างเดียว)</summary>
    public Task<Announcement?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Announcements.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    /// <summary>ค้นหาประกาศตามรหัส (ติดตามการเปลี่ยนแปลง)</summary>
    public Task<Announcement?> FindByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Announcements.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    /// <summary>เพิ่มประกาศใหม่</summary>
    public async Task AddAsync(Announcement announcement, CancellationToken cancellationToken = default) =>
        await db.Announcements.AddAsync(announcement, cancellationToken);

    /// <summary>บันทึกการเปลี่ยนแปลงลงฐานข้อมูล</summary>
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}

