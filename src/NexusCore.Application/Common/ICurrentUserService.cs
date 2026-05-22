namespace NexusCore.Application.Common;

/// <summary>
/// ข้อมูลผู้ใช้ที่ล็อกอินอยู่และบทบาท สำหรับตรวจสอบสิทธิ์ใน use case
/// </summary>
public interface ICurrentUserService
{
    /// <summary>รหัสผู้ใช้ปัจจุบัน (null ถ้ายังไม่ล็อกอิน)</summary>
    Guid? UserId { get; }

    /// <summary>ชื่อบทบาทปัจจุบัน</summary>
    string? Role { get; }

    /// <summary>ตรวจว่าผู้ใช้อยู่ในบทบาทที่ระบุหรือไม่</summary>
    bool IsInRole(string role);

    /// <summary>ตรวจว่าผู้ใช้อยู่ในบทบาทใดบทบาทหนึ่งที่ระบุหรือไม่</summary>
    bool IsInAnyRole(params string[] roles);
}
