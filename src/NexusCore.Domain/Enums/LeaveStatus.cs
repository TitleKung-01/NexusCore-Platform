namespace NexusCore.Domain.Enums;

/// <summary>
/// สถานะ workflow ของคำขอที่ต้องอนุมัติ (ลา OT เบิกจ่าย) — ใช้ร่วมกันเพื่อ logic อนุมัติเดียวกัน
/// </summary>
public enum LeaveStatus
{
    /// <summary>ร่าง — ยังไม่ส่งให้ผู้อนุมัติ</summary>
    Draft = 0,
    /// <summary>รออนุมัติ — ส่งแล้วรอผู้จัดการ/HR</summary>
    Pending = 1,
    /// <summary>อนุมัติแล้ว</summary>
    Approved = 2,
    /// <summary>ปฏิเสธแล้ว</summary>
    Rejected = 3,
    /// <summary>ยกเลิกโดยผู้ขอหรือระบบ</summary>
    Cancelled = 4
}
