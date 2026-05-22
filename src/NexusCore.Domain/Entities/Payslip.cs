namespace NexusCore.Domain.Entities;

/// <summary>
/// สลิปเงินเดือนที่ HR อัปโหลดให้พนักงานดาวน์โหลด — เก็บเมตาดาตาและตำแหน่งไฟล์จริง
/// </summary>
public class Payslip
{
    /// <summary>รหัสสลิป</summary>
    public Guid Id { get; set; }
    /// <summary>รหัสพนักงานเจ้าของสลิป</summary>
    public Guid EmployeeId { get; set; }
    /// <summary>โปรไฟล์พนักงานเจ้าของสลิป</summary>
    public EmployeeProfile Employee { get; set; } = null!;
    /// <summary>ปีที่ออกสลิป (ค.ศ.)</summary>
    public int Year { get; set; }
    /// <summary>เดือนที่ออกสลิป (1–12)</summary>
    public int Month { get; set; }
    /// <summary>ชื่อไฟล์ต้นฉบับที่อัปโหลด</summary>
    public string FileName { get; set; } = string.Empty;
    /// <summary>เส้นทางเก็บไฟล์ในระบบจัดเก็บ (เช่น โฟลเดอร์ uploads)</summary>
    public string StoragePath { get; set; } = string.Empty;
    /// <summary>วันเวลาที่เผยแพร่สลิป (UTC)</summary>
    public DateTime PublishedAtUtc { get; set; }
    /// <summary>รหัสผู้ใช้ที่อัปโหลด/เผยแพร่สลิป</summary>
    public Guid PublishedById { get; set; }
}
