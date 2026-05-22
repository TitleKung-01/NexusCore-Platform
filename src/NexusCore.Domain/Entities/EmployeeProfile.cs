namespace NexusCore.Domain.Entities;

/// <summary>
/// โปรไฟล์พนักงานที่เชื่อมกับบัญชีผู้ใช้ — เก็บข้อมูล HR สำหรับแสดงผล อนุมัติคำขอ และจัดโครงสร้างองค์กร
/// </summary>
public class EmployeeProfile
{
    /// <summary>รหัสผู้ใช้ที่ผูกกับโปรไฟล์นี้ (คีย์หลักร่วมกับ User)</summary>
    public Guid UserId { get; set; }
    /// <summary>บัญชีเข้าสู่ระบบของพนักงาน</summary>
    public User User { get; set; } = null!;
    /// <summary>ชื่อ-นามสกุลที่แสดงในระบบ</summary>
    public string FullName { get; set; } = string.Empty;
    /// <summary>อีเมลติดต่อ (อาจซ้ำกับ Username แต่ใช้สำหรับการสื่อสาร HR)</summary>
    public string Email { get; set; } = string.Empty;
    /// <summary>เบอร์โทรศัพท์ (ไม่บังคับ)</summary>
    public string? Phone { get; set; }
    /// <summary>รหัสแผนกที่สังกัดอยู่</summary>
    public Guid DepartmentId { get; set; }
    /// <summary>แผนกปัจจุบันของพนักงาน</summary>
    public Department Department { get; set; } = null!;
    /// <summary>รหัสผู้จัดการโดยตรง (ถ้ามี) — ใช้สำหรับสายอนุมัติ</summary>
    public Guid? ManagerId { get; set; }
    /// <summary>โปรไฟล์ผู้จัดการโดยตรง</summary>
    public EmployeeProfile? Manager { get; set; }
}
