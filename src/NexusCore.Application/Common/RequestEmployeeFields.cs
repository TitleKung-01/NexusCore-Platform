using NexusCore.Domain.Entities;

namespace NexusCore.Application.Common;

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
