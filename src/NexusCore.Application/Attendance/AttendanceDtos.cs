namespace NexusCore.Application.Attendance;

public record AttendanceResponse(
    Guid Id,
    Guid EmployeeId,
    string EmployeeName,
    string WorkDate,
    string? CheckInUtc,
    string? CheckOutUtc,
    string? CheckInLocal,
    string? CheckOutLocal,
    bool IsLateCheckIn,
    int LateMinutes,
    bool IsEarlyCheckOut,
    string StatusLabel,
    string? WorkSummary,
    bool CanCheckIn,
    bool CanCheckOut);

public record CheckInRequest(string WorkDate);

public record CheckOutRequest(string WorkDate, string? WorkSummary);
