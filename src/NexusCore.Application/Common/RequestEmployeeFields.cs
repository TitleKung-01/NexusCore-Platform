using NexusCore.Domain.Entities;

namespace NexusCore.Application.Common;

/// <summary>
/// ดึงฟิลด์พนักงานจากโปรไฟล์สำหรับแสดงในคำขอลา/โอที/เบิกจ่าย
/// </summary>
internal static class RequestEmployeeFields
{
    internal static (
        string Username,
        string Email,
        string DepartmentName,
        string Role,
        string? ManagerName) From(EmployeeProfile employee) =>
        (
            employee.User?.Username ?? string.Empty,
            employee.Email,
            employee.Department?.Name ?? string.Empty,
            employee.User?.Role ?? string.Empty,
            employee.Manager?.FullName);
}
