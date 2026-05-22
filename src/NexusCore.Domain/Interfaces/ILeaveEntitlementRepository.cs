using NexusCore.Domain.Entities;

namespace NexusCore.Domain.Interfaces;

/// <summary>
/// สัญญาเข้าถึงโควตาวันลา — ใช้ตรวจสอบสิทธิ์ก่อนอนุมัติและแสดงยอดคงเหลือ
/// </summary>
public interface ILeaveEntitlementRepository
{
    /// <summary>ดึงโควตาเฉพาะพนักงาน ประเภทลา และปี</summary>
    Task<LeaveEntitlement?> GetAsync(Guid employeeId, Guid leaveTypeId, int year, CancellationToken cancellationToken = default);
    /// <summary>รายการโควตาทั้งหมดของพนักงาน (กรองปีได้)</summary>
    Task<IReadOnlyList<LeaveEntitlement>> ListForEmployeeAsync(Guid employeeId, int? year, CancellationToken cancellationToken = default);
    /// <summary>เพิ่มหรืออัปเดตโควตาใหม่</summary>
    Task AddAsync(LeaveEntitlement entitlement, CancellationToken cancellationToken = default);
    /// <summary>บันทึกการเปลี่ยนแปลงลงฐานข้อมูล</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
