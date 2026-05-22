namespace NexusCore.Application.Employees;

/// <summary>โปรไฟล์พนักงานของผู้ล็อกอิน พร้อมสิทธิ์อนุมัติ/HR</summary>
public record MeResponse(
    Guid UserId,
    string Username,
    string Role,
    string FullName,
    string Email,
    string? Phone,
    Guid DepartmentId,
    string DepartmentName,
    Guid? ManagerId,
    string? ManagerName,
    bool CanApprove,
    bool IsHrAccess);

/// <summary>คำขอแก้ไขข้อมูลส่วนตัวของตนเอง</summary>
public record UpdateMeRequest(string FullName, string Email, string? Phone);

/// <summary>รายการพนักงานสำหรับ HR</summary>
public record EmployeeListItem(
    Guid UserId,
    string Username,
    string Role,
    string FullName,
    string Email,
    string DepartmentName,
    string? ManagerName);

/// <summary>คำขอ HR แก้ไขแผนก ผู้จัดการ บทบาท และสถานะพนักงาน</summary>
public record UpdateEmployeeRequest(
    Guid? DepartmentId,
    Guid? ManagerId,
    string? Role,
    string? FullName,
    string? Email,
    bool? IsActive);

/// <summary>ข้อมูลแผนกองค์กร</summary>
public record DepartmentResponse(Guid Id, string Name, string Code);

/// <summary>ประเภทการลาที่ใช้ในระบบ</summary>
public record LeaveTypeResponse(Guid Id, string Name, string Code);
