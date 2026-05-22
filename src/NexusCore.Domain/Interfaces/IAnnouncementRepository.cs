using NexusCore.Domain.Entities;

namespace NexusCore.Domain.Interfaces;

/// <summary>
/// สัญญาเข้าถึงประกาศข่าวสาร — แยกการอ่านบนแดชบอร์ดกับการจัดการโดย HR
/// </summary>
public interface IAnnouncementRepository
{
    /// <summary>รายการประกาศที่ยังเปิดใช้งาน</summary>
    Task<IReadOnlyList<Announcement>> ListActiveAsync(CancellationToken cancellationToken = default);
    /// <summary>ค้นหาประกาศตามรหัส (อ่านอย่างเดียว)</summary>
    Task<Announcement?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    /// <summary>ค้นหาประกาศพร้อมติดตามการแก้ไข</summary>
    Task<Announcement?> FindByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default);
    /// <summary>เพิ่มประกาศใหม่</summary>
    Task AddAsync(Announcement announcement, CancellationToken cancellationToken = default);
    /// <summary>บันทึกการเปลี่ยนแปลงลงฐานข้อมูล</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
