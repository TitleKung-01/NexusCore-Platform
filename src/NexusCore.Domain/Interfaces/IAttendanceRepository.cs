using NexusCore.Domain.Entities;

namespace NexusCore.Domain.Interfaces;

/// <summary>
/// สัญญาเข้าถึงบันทึกการลงเวลา — เช็คอิน/เอาท์และรายงานช่วงวันที่
/// </summary>
public interface IAttendanceRepository
{
    /// <summary>ค้นหาบันทึกตามรหัส</summary>
    Task<AttendanceRecord?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    /// <summary>ค้นหาบันทึกของพนักงานในวันนั้น (อ่านอย่างเดียว)</summary>
    Task<AttendanceRecord?> FindByEmployeeAndDateAsync(Guid employeeId, DateOnly workDate, CancellationToken cancellationToken = default);
    /// <summary>ค้นหาบันทึกของพนักงานในวันนั้น พร้อมติดตามการแก้ไข</summary>
    Task<AttendanceRecord?> FindByEmployeeAndDateTrackedAsync(Guid employeeId, DateOnly workDate, CancellationToken cancellationToken = default);
    /// <summary>รายการลงเวลาในช่วงวันที่ (กรองตามพนักงานได้)</summary>
    Task<IReadOnlyList<AttendanceRecord>> ListAsync(Guid? employeeId, DateOnly from, DateOnly to, CancellationToken cancellationToken = default);
    /// <summary>เพิ่มบันทึกลงเวลาใหม่</summary>
    Task AddAsync(AttendanceRecord record, CancellationToken cancellationToken = default);
    /// <summary>บันทึกการเปลี่ยนแปลงลงฐานข้อมูล</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
