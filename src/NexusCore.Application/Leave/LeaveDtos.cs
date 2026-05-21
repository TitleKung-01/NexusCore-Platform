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

public record LeaveBalanceResponse(
    Guid LeaveTypeId,
    string LeaveTypeName,
    int Year,
    decimal DaysAllowed,
    decimal DaysUsed,
    decimal DaysRemaining);

public record LeaveCalendarEntryResponse(
    Guid Id,
    Guid EmployeeId,
    string EmployeeName,
    string LeaveTypeName,
    string StartDate,
    string EndDate,
    string Status);

public record LeaveAttachmentResponse(
    Guid Id,
    Guid LeaveRequestId,
    string FileName,
    string ContentType,
    long SizeBytes,
    string UploadedAtUtc);
