using NexusCore.Application.Common;

namespace NexusCore.Application.Employees;

/// <summary>
/// บริการโปรไฟล์พนักงาน แผนก ประเภทลา และรายชื่อพนักงานสำหรับ HR
/// </summary>
public interface IEmployeeService
{
    /// <summary>ดึงโปรไฟล์ของผู้ใช้ที่ล็อกอิน</summary>
    Task<MeResponse?> GetMeAsync(CancellationToken cancellationToken = default);

    /// <summary>อัปเดตข้อมูลส่วนตัวของตนเอง</summary>
    Task<ServiceResult<MeResponse>> UpdateMeAsync(UpdateMeRequest request, CancellationToken cancellationToken = default);

    /// <summary>รายการแผนกทั้งหมด</summary>
    Task<IReadOnlyList<DepartmentResponse>> GetDepartmentsAsync(CancellationToken cancellationToken = default);

    /// <summary>รายการประเภทการลา</summary>
    Task<IReadOnlyList<LeaveTypeResponse>> GetLeaveTypesAsync(CancellationToken cancellationToken = default);

    /// <summary>รายชื่อพนักงาน (เฉพาะ HR/Admin)</summary>
    Task<IReadOnlyList<EmployeeListItem>> GetEmployeesAsync(CancellationToken cancellationToken = default);

    /// <summary>HR อัปเดตข้อมูลพนักงานและบันทึกการย้ายแผนก</summary>
    Task<ServiceResult<EmployeeListItem>> UpdateEmployeeAsync(Guid userId, UpdateEmployeeRequest request, CancellationToken cancellationToken = default);
}
