namespace NexusCore.Domain.Entities;

/// <summary>
/// ประกาศข่าวสารจาก HR/ผู้ดูแล — แสดงบนแดชบอร์ดเมื่อยังเปิดใช้งาน
/// </summary>
public class Announcement
{
    /// <summary>รหัสประกาศ</summary>
    public Guid Id { get; set; }
    /// <summary>หัวข้อประกาศ</summary>
    public string Title { get; set; } = string.Empty;
    /// <summary>เนื้อหาประกาศ (ข้อความหรือ HTML ตามที่แอปรองรับ)</summary>
    public string Body { get; set; } = string.Empty;
    /// <summary>รหัสผู้ใช้ที่สร้าง/เผยแพร่ประกาศ</summary>
    public Guid AuthorId { get; set; }
    /// <summary>วันเวลาเผยแพร่ (UTC)</summary>
    public DateTime PublishedAtUtc { get; set; }
    /// <summary>สถานะแสดงผล — false คือซ่อนจากผู้ใช้ทั่วไป</summary>
    public bool IsActive { get; set; } = true;
}
