using NexusCore.Domain.Entities;

namespace NexusCore.Domain.Interfaces;

/// <summary>
/// สัญญาเข้าถึงบัญชีผู้ใช้ — ใช้ล็อกอิน จัดการผู้ใช้ และ seed ข้อมูลเริ่มต้น
/// </summary>
public interface IUserRepository
{
    /// <summary>ค้นหาผู้ใช้จากชื่อผู้ใช้ (ล็อกอิน)</summary>
    Task<User?> FindByUsernameAsync(string username, CancellationToken cancellationToken = default);
    /// <summary>ค้นหาผู้ใช้จากรหัส</summary>
    Task<User?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    /// <summary>ตรวจว่ามีชื่อผู้ใช้นี้แล้วหรือไม่ — ป้องกันซ้ำตอนสร้างบัญชี</summary>
    Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default);
    /// <summary>ตรวจว่ามีผู้ใช้ในระบบอย่างน้อยหนึ่งคน (ใช้ตัดสิน seed)</summary>
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
    /// <summary>รายการผู้ใช้ทั้งหมดเรียงตามชื่อผู้ใช้</summary>
    Task<IReadOnlyList<User>> GetAllOrderedAsync(CancellationToken cancellationToken = default);
    /// <summary>ค้นหาผู้ใช้พร้อมติดตามการแก้ไข</summary>
    Task<User?> FindByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default);
    /// <summary>เพิ่มบัญชีผู้ใช้ใหม่</summary>
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    /// <summary>ลบผู้ใช้ออกจาก context (บันทึกเมื่อ SaveChanges)</summary>
    void Remove(User user);
    /// <summary>บันทึกการเปลี่ยนแปลงลงฐานข้อมูล</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
