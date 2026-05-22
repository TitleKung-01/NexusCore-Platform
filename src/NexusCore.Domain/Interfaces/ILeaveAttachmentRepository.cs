using NexusCore.Domain.Entities;

namespace NexusCore.Domain.Interfaces;

/// <summary>
/// สัญญาเข้าถึงไฟล์แนบคำขอลา — แยกการจัดเก็บไฟล์ออกจาก entity หลัก
/// </summary>
public interface ILeaveAttachmentRepository
{
    /// <summary>ค้นหาไฟล์แนบตามรหัส</summary>
    Task<LeaveAttachment?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    /// <summary>รายการไฟล์แนบทั้งหมดของคำขอลาหนึ่งรายการ</summary>
    Task<IReadOnlyList<LeaveAttachment>> ListByLeaveRequestIdAsync(Guid leaveRequestId, CancellationToken cancellationToken = default);
    /// <summary>เพิ่มไฟล์แนบใหม่</summary>
    Task AddAsync(LeaveAttachment attachment, CancellationToken cancellationToken = default);
    /// <summary>บันทึกการเปลี่ยนแปลงลงฐานข้อมูล</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
