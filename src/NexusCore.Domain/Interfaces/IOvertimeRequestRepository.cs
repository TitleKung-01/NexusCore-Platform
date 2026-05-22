using NexusCore.Domain.Entities;

namespace NexusCore.Domain.Interfaces;

/// <summary>
/// สัญญาเข้าถึงคำขอทำงานล่วงเวลา — รองรับมุมมองพนักงาน ผู้จัดการ และ HR
/// </summary>
public interface IOvertimeRequestRepository
{
    /// <summary>ค้นหาคำขอ OT ตามรหัส (อ่านอย่างเดียว)</summary>
    Task<OvertimeRequest?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    /// <summary>ค้นหาคำขอ OT พร้อมติดตามการแก้ไข</summary>
    Task<OvertimeRequest?> FindByIdTrackedAsync(Guid id, CancellationToken cancellationToken = default);
    /// <summary>รายการคำขอ OT ของพนักงานคนหนึ่ง</summary>
    Task<IReadOnlyList<OvertimeRequest>> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default);
    /// <summary>คำขอที่รออนุมัติของลูกน้องภายใต้ผู้จัดการ</summary>
    Task<IReadOnlyList<OvertimeRequest>> GetPendingForManagerAsync(Guid managerUserId, CancellationToken cancellationToken = default);
    /// <summary>คำขอที่รออนุมัติทั้งองค์กร (มุมมอง HR)</summary>
    Task<IReadOnlyList<OvertimeRequest>> GetAllPendingAsync(CancellationToken cancellationToken = default);
    /// <summary>ประวัติการอนุมัติ/ปฏิเสธ (กรองตามผู้จัดการได้)</summary>
    Task<IReadOnlyList<OvertimeRequest>> GetApprovalHistoryAsync(Guid? managerId, CancellationToken cancellationToken = default);
    /// <summary>เพิ่มคำขอ OT ใหม่</summary>
    Task AddAsync(OvertimeRequest request, CancellationToken cancellationToken = default);
    /// <summary>บันทึกการเปลี่ยนแปลงลงฐานข้อมูล</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
