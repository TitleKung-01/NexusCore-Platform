namespace NexusCore.Application.Overtime;

public record OvertimeRequestResponse(
    Guid Id,
    Guid EmployeeId,
    string EmployeeName,
    string EmployeeUsername,
    string EmployeeEmail,
    string EmployeeDepartment,
    string EmployeeRole,
    string? EmployeeManagerName,
    string WorkDate,
    decimal Hours,
    string Reason,
    string Status,
    string? SubmittedAtUtc,
    string? DecidedAtUtc,
    string? ManagerComment);

public record CreateOvertimeRequest(string WorkDate, decimal Hours, string Reason);

public record DecideOvertimeRequest(string? Comment);
