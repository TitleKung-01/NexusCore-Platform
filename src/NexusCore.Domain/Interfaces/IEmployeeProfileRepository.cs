using NexusCore.Domain.Entities;

namespace NexusCore.Domain.Interfaces;

/// <summary>
/// สัญญาเข้าถึงโปรไฟล์พนักงาน — ใช้เชื่อมบัญชีผู้ใช้กับข้อมูล HR และโครงสร้างทีม
/// </summary>
public interface IEmployeeProfileRepository
{
    /// <summary>ค้นหาโปรไฟล์จากรหัสผู้ใช้ (อ่านอย่างเดียว)</summary>
    Task<EmployeeProfile?> FindByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    /// <summary>ค้นหาโปรไฟล์จากรหัสผู้ใช้ พร้อมติดตามการแก้ไข</summary>
    Task<EmployeeProfile?> FindByUserIdTrackedAsync(Guid userId, CancellationToken cancellationToken = default);
    /// <summary>รายการพนักงานทั้งหมดพร้อมแผนกและผู้จัดการ</summary>
    Task<IReadOnlyList<EmployeeProfile>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);
    /// <summary>เพิ่มโปรไฟล์พนักงานใหม่</summary>
    Task AddAsync(EmployeeProfile profile, CancellationToken cancellationToken = default);
    /// <summary>บันทึกการเปลี่ยนแปลงลงฐานข้อมูล</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
