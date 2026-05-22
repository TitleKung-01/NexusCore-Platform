using NexusCore.Domain.Entities;

namespace NexusCore.Domain.Interfaces;

/// <summary>
/// สัญญาเข้าถึงแผนก — ใช้จัดโครงสร้างองค์กรและอ้างอิงในฟอร์ม HR
/// </summary>
public interface IDepartmentRepository
{
    /// <summary>รายการแผนกทั้งหมด</summary>
    Task<IReadOnlyList<Department>> GetAllAsync(CancellationToken cancellationToken = default);
    /// <summary>ค้นหาแผนกตามรหัส</summary>
    Task<Department?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    /// <summary>ค้นหาแผนกตามรหัสย่อ (ไม่ซ้ำ)</summary>
    Task<Department?> FindByCodeAsync(string code, CancellationToken cancellationToken = default);
    /// <summary>เพิ่มแผนกใหม่</summary>
    Task AddAsync(Department department, CancellationToken cancellationToken = default);
    /// <summary>บันทึกการเปลี่ยนแปลงลงฐานข้อมูล</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
