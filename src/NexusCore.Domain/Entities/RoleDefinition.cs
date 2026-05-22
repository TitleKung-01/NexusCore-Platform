namespace NexusCore.Domain.Entities;

/// <summary>
/// นิยามบทบาทที่กำหนดสิทธิ์อนุมัติและการเข้าถึง HR — แยกจากสตริง Role บน User เพื่อปรับนโยบายได้
/// </summary>
public class RoleDefinition
{
    /// <summary>รหัสนิยามบทบาท</summary>
    public Guid Id { get; set; }
    /// <summary>ชื่อบทบาท (ตรงกับค่า Role บนบัญชีผู้ใช้)</summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>คำอธิบายบทบาท (ไม่บังคับ)</summary>
    public string? Description { get; set; }
    /// <summary>อนุญาตให้อนุมัติคำขอของลูกน้องหรือไม่</summary>
    public bool CanApprove { get; set; }
    /// <summary>อนุญาตเข้าถึงฟีเจอร์ HR หรือไม่</summary>
    public bool IsHrAccess { get; set; }
    /// <summary>บทบาทที่ระบบสร้างไว้ล่วงหน้า — มักห้ามลบ</summary>
    public bool IsBuiltIn { get; set; }
}
