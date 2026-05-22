namespace NexusCore.Domain.Entities;

/// <summary>
/// บันทึกการลงเวลาเข้า-ออกต่อวันของพนักงาน — ใช้คำนวณชั่วโมงทำงานและสรุปงานรายวัน
/// </summary>
public class AttendanceRecord
{
    /// <summary>รหัสบันทึก</summary>
    public Guid Id { get; set; }
    /// <summary>รหัสพนักงานที่ลงเวลา</summary>
    public Guid EmployeeId { get; set; }
    /// <summary>โปรไฟล์พนักงานที่ลงเวลา</summary>
    public EmployeeProfile Employee { get; set; } = null!;
    /// <summary>วันที่ทำงาน (ตามปฏิทินท้องถิ่น)</summary>
    public DateOnly WorkDate { get; set; }
    /// <summary>เวลาเช็คอิน (UTC) ถ้ามี</summary>
    public DateTime? CheckInUtc { get; set; }
    /// <summary>เวลาเช็คเอาท์ (UTC) ถ้ามี</summary>
    public DateTime? CheckOutUtc { get; set; }
    /// <summary>สรุปงานวันนี้ (ไม่บังคับ) — มักกรอกตอนลงเวลาออก</summary>
    public string? WorkSummary { get; set; }
}
