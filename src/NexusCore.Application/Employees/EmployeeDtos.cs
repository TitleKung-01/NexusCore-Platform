namespace NexusCore.Application.Employees;

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
    string? ManagerName);

public record UpdateMeRequest(string FullName, string Email, string? Phone);

public record EmployeeListItem(
    Guid UserId,
    string Username,
    string Role,
    string FullName,
    string Email,
    string DepartmentName,
    string? ManagerName);

public record UpdateEmployeeRequest(
    Guid? DepartmentId,
    Guid? ManagerId,
    string? Role,
    string? FullName,
    string? Email,
    bool? IsActive);

public record DepartmentResponse(Guid Id, string Name, string Code);

public record LeaveTypeResponse(Guid Id, string Name, string Code);
