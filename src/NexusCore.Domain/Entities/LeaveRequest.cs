using NexusCore.Domain.Enums;

namespace NexusCore.Domain.Entities;

/// <summary>
/// คำขอลาของพนักงาน — ผ่าน workflow ร่าง ส่งอนุมัติ และตัดสินโดยผู้จัดการ/HR
/// </summary>
public class LeaveRequest
{
    /// <summary>รหัสคำขอ</summary>
    public Guid Id { get; set; }
    /// <summary>รหัสพนักงานผู้ขอ</summary>
    public Guid EmployeeId { get; set; }
    /// <summary>โปรไฟล์ผู้ขอ</summary>
    public EmployeeProfile Employee { get; set; } = null!;
    /// <summary>รหัสประเภทลา</summary>
    public Guid LeaveTypeId { get; set; }
    /// <summary>ประเภทลาที่ขอ</summary>
    public LeaveType LeaveType { get; set; } = null!;
    /// <summary>วันเริ่มลา</summary>
    public DateOnly StartDate { get; set; }
    /// <summary>วันสิ้นสุดลา (รวมวันนี้)</summary>
    public DateOnly EndDate { get; set; }
    /// <summary>เหตุผลการลา</summary>
    public string Reason { get; set; } = string.Empty;
    /// <summary>สถานะ workflow ปัจจุบัน</summary>
    public LeaveStatus Status { get; set; } = LeaveStatus.Draft;
    /// <summary>วันเวลาส่งคำขอ (UTC) เมื่อเปลี่ยนจากร่าง</summary>
    public DateTime? SubmittedAtUtc { get; set; }
    /// <summary>วันเวลาที่มีการอนุมัติ/ปฏิเสธ (UTC)</summary>
    public DateTime? DecidedAtUtc { get; set; }
    /// <summary>รหัสผู้ตัดสิน (ผู้จัดการ/HR)</summary>
    public Guid? DecidedById { get; set; }
    /// <summary>ความเห็นจากผู้อนุมัติ (ไม่บังคับ)</summary>
    public string? ManagerComment { get; set; }
    /// <summary>วันเวลาสร้างคำขอ (UTC)</summary>
    public DateTime CreatedAtUtc { get; set; }
}
