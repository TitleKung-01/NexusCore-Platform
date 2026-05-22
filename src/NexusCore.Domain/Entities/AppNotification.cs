namespace NexusCore.Domain.Entities;

/// <summary>
/// การแจ้งเตือนในแอปสำหรับผู้ใช้คนเดียว — สร้างเมื่อมีเหตุการณ์สำคัญ (อนุมัติ ปฏิเสธ ฯลฯ)
/// </summary>
public class AppNotification
{
    /// <summary>รหัสการแจ้งเตือน</summary>
    public Guid Id { get; set; }
    /// <summary>รหัสผู้ใช้ผู้รับ</summary>
    public Guid UserId { get; set; }
    /// <summary>ประเภทเหตุการณ์ (ใช้แยกไอคอน/เทมเพลตข้อความ)</summary>
    public string EventType { get; set; } = string.Empty;
    /// <summary>หัวข้อสั้น</summary>
    public string Title { get; set; } = string.Empty;
    /// <summary>เนื้อหาแจ้งเตือน</summary>
    public string Body { get; set; } = string.Empty;
    /// <summary>เส้นทางในแอปที่คลิกแล้วไปต่อ (ไม่บังคับ)</summary>
    public string? LinkPath { get; set; }
    /// <summary>ผู้ใช้อ่านแล้วหรือยัง</summary>
    public bool IsRead { get; set; }
    /// <summary>วันเวลาสร้าง (UTC)</summary>
    public DateTime CreatedAtUtc { get; set; }
}
