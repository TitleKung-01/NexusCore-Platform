using NexusCore.Domain.Enums;

namespace NexusCore.Domain.Entities;

/// <summary>
/// คำขอทำงานล่วงเวลา — ใช้ workflow สถานะเดียวกับลา/เบิกจ่ายเพื่อให้ผู้จัดการอนุมัติ
/// </summary>
public class OvertimeRequest
{
    /// <summary>รหัสคำขอ OT</summary>
    public Guid Id { get; set; }
    /// <summary>รหัสพนักงานผู้ขอ</summary>
    public Guid EmployeeId { get; set; }
    /// <summary>โปรไฟล์ผู้ขอ</summary>
    public EmployeeProfile Employee { get; set; } = null!;
    /// <summary>วันที่ทำ OT</summary>
    public DateOnly WorkDate { get; set; }
    /// <summary>จำนวนชั่วโมง OT ที่ขอ</summary>
    public decimal Hours { get; set; }
    /// <summary>เหตุผลการทำ OT</summary>
    public string Reason { get; set; } = string.Empty;
    /// <summary>สถานะ workflow ปัจจุบัน</summary>
    public LeaveStatus Status { get; set; } = LeaveStatus.Draft;
    /// <summary>วันเวลาส่งคำขอ (UTC)</summary>
    public DateTime? SubmittedAtUtc { get; set; }
    /// <summary>วันเวลาที่มีการอนุมัติ/ปฏิเสธ (UTC)</summary>
    public DateTime? DecidedAtUtc { get; set; }
    /// <summary>รหัสผู้ตัดสิน</summary>
    public Guid? DecidedById { get; set; }
    /// <summary>ความเห็นจากผู้อนุมัติ (ไม่บังคับ)</summary>
    public string? ManagerComment { get; set; }
    /// <summary>วันเวลาสร้างคำขอ (UTC)</summary>
    public DateTime CreatedAtUtc { get; set; }
}
