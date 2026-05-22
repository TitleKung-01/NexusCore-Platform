namespace NexusCore.Domain.Entities;

/// <summary>
/// บัญชีผู้ใช้สำหรับเข้าสู่ระบบ — แยกจากโปรไฟล์พนักงานเพื่อรองรับบทบาทและการยืนยันตัวตน
/// </summary>
public class User
{
    /// <summary>รหัสผู้ใช้</summary>
    public Guid Id { get; set; }
    /// <summary>ชื่อผู้ใช้สำหรับล็อกอิน (มักเป็นอีเมล)</summary>
    public string Username { get; set; } = string.Empty;
    /// <summary>รหัสผ่านที่แฮชแล้ว — ไม่เก็บรหัสผ่านต้นฉบับ</summary>
    public string PasswordHash { get; set; } = string.Empty;
    /// <summary>บทบาทในระบบ (อ้างอิงค่าจาก UserRoles)</summary>
    public string Role { get; set; } = string.Empty;
    /// <summary>สถานะบัญชี — ปิดใช้งานแล้วจะล็อกอินไม่ได้</summary>
    public bool IsActive { get; set; } = true;
    /// <summary>วันเวลาสร้างบัญชี (UTC)</summary>
    public DateTime CreatedAtUtc { get; set; }
    /// <summary>โปรไฟล์พนักงานที่ผูกกับบัญชีนี้ (ถ้ามี)</summary>
    public EmployeeProfile? Profile { get; set; }
}
