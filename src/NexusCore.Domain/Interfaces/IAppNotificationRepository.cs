using NexusCore.Domain.Entities;

namespace NexusCore.Domain.Interfaces;

/// <summary>
/// สัญญาเข้าถึงการแจ้งเตือนในแอป — แยก Domain จาก EF เพื่อทดสอบและสลับที่เก็บข้อมูลได้
/// </summary>
public interface IAppNotificationRepository
{
    /// <summary>ดึงรายการแจ้งเตือนล่าสุดของผู้ใช้ (จำกัดจำนวน)</summary>
    Task<IReadOnlyList<AppNotification>> GetForUserAsync(Guid userId, int limit, CancellationToken cancellationToken = default);
    /// <summary>นับจำนวนที่ยังไม่อ่าน — แสดง badge บน UI</summary>
    Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default);
    /// <summary>เพิ่มการแจ้งเตือนใหม่ (ยังไม่บันทึกจนกว่า SaveChanges)</summary>
    Task AddAsync(AppNotification notification, CancellationToken cancellationToken = default);
    /// <summary>ค้นหาแจ้งเตือนพร้อมติดตามการเปลี่ยนแปลง ของผู้ใช้ที่ระบุ</summary>
    Task<AppNotification?> FindByIdTrackedAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    /// <summary>ทำเครื่องหมายว่าอ่านทั้งหมดของผู้ใช้</summary>
    Task MarkAllReadAsync(Guid userId, CancellationToken cancellationToken = default);
    /// <summary>บันทึกการเปลี่ยนแปลงลงฐานข้อมูล</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
