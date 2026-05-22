namespace NexusCore.Domain.Constants;

/// <summary>
/// ค่าคงที่ชื่อบทบาทผู้ใช้ — กำหนดมาตรฐานเดียวทั้งระบบเพื่อตรวจสิทธิ์และ seed ข้อมูล
/// </summary>
public static class UserRoles
{
    /// <summary>พนักงานทั่วไป — ใช้ฟีเจอร์ส่วนตัว</summary>
    public const string Employee = "Employee";
    /// <summary>ผู้จัดการ — อนุมัติลูกน้องได้</summary>
    public const string Manager = "Manager";
    /// <summary>ฝ่าย HR — จัดการข้อมูลและนโยบาย HR</summary>
    public const string Hr = "Hr";
    /// <summary>ผู้ดูแลระบบ — สิทธิ์สูงสุดรวมการตั้งค่า</summary>
    public const string Admin = "Admin";

    /// <summary>บทบาททั้งหมดที่ระบบรองรับ</summary>
    public static readonly string[] All = [Employee, Manager, Hr, Admin];
    /// <summary>บทบาทที่มีสิทธิ์อนุมัติคำขอ</summary>
    public static readonly string[] Approvers = [Manager, Hr, Admin];
    /// <summary>บทบาทที่เข้าถึงฟีเจอร์ HR หรือผู้ดูแล</summary>
    public static readonly string[] HrOrAdmin = [Hr, Admin];
}
