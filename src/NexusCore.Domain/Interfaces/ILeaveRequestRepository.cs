using NexusCore.Domain.Entities;

namespace NexusCore.Domain.Interfaces;

/// <summary>
/// สัญญาเข้าถึงคำขอลา — รองรับ workflow อนุมัติ ปฏิทิน และตรวจโควตา/ช่วงวันที่ซ้อน
/// </summary>
public interface ILeaveRequestRepository
{
    /// <summary>ค้นหาคำขอลาตามรหัส (อ่านอย่างเดียว)</summary>
    Task<LeaveRequest?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    /// <summary>ค้นหาคำขอลาพร้อมติดตามการแก้ไข</summary>
    Task<LeaveRequest?> FindByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default);
    /// <summary>รายการคำขอลาของพนักงานคนหนึ่ง</summary>
    Task<IReadOnlyList<LeaveRequest>> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default);
    /// <summary>คำขอที่รออนุมัติของลูกน้องภายใต้ผู้จัดการ</summary>
    Task<IReadOnlyList<LeaveRequest>> GetPendingForManagerAsync(Guid managerUserId, CancellationToken cancellationToken = default);
    /// <summary>คำขอที่รออนุมัติทั้งองค์กร (มุมมอง HR)</summary>
    Task<IReadOnlyList<LeaveRequest>> GetAllPendingAsync(CancellationToken cancellationToken = default);
    /// <summary>คำขอลาในช่วงวันที่สำหรับปฏิทิน (กรองแผนกได้)</summary>
    Task<IReadOnlyList<LeaveRequest>> GetCalendarAsync(DateOnly from, DateOnly to, Guid? departmentId, CancellationToken cancellationToken = default);
    /// <summary>ประวัติการอนุมัติ/ปฏิเสธ (กรองตามผู้จัดการได้)</summary>
    Task<IReadOnlyList<LeaveRequest>> GetApprovalHistoryAsync(Guid? managerId, CancellationToken cancellationToken = default);
    /// <summary>รวมจำนวนวันลาที่อนุมัติแล้วในปีนั้น — ใช้เทียบกับโควตา</summary>
    Task<decimal> SumApprovedDaysAsync(Guid employeeId, Guid leaveTypeId, int year, CancellationToken cancellationToken = default);
    /// <summary>ตรวจว่ามีคำขอที่ช่วงวันที่ทับซ้อนหรือไม่ (ยกเว้นคำขอที่กำลังแก้ได้)</summary>
    Task<bool> HasOverlappingAsync(Guid employeeId, DateOnly start, DateOnly end, Guid? excludeId, CancellationToken cancellationToken = default);
    /// <summary>เพิ่มคำขอลาใหม่</summary>
    Task AddAsync(LeaveRequest request, CancellationToken cancellationToken = default);
    /// <summary>บันทึกการเปลี่ยนแปลงลงฐานข้อมูล</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
