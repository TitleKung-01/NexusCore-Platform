namespace NexusCore.Application.Leave;

public record LeaveRequestResponse(
    Guid Id,
    Guid EmployeeId,
    string EmployeeName,
    Guid LeaveTypeId,
    string LeaveTypeName,
    string StartDate,
    string EndDate,
    string Reason,
    string Status,
    string? SubmittedAtUtc,
    string? DecidedAtUtc,
    string? ManagerComment);

public record CreateLeaveRequest(Guid LeaveTypeId, string StartDate, string EndDate, string Reason);

public record DecideLeaveRequest(string? Comment);
